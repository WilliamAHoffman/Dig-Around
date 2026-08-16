using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkManager : MonoBehaviour
{
    #region Inspector References

    [Header("Tilemaps")]
    [SerializeField] private Tilemap walls;
    [SerializeField] private Tilemap floors;

    [Header("Generation")]
    [SerializeField] private WorldGenerator generator;

    [Header("References")]
    [SerializeField] private GameController gameController;

    #endregion

    #region Runtime Data

    private readonly Dictionary<Vector2Int, Chunk> chunks = new();

    private readonly Queue<Vector2Int> queuedChunks = new();
    private readonly HashSet<Vector2Int> queuedChunkSet = new();

    private readonly Dictionary<Vector2Int, bool> renderRequests = new();

    private readonly Dictionary<int, TileDataAsset> tileCache = new();

    private Coroutine loadingCoroutine;

    #endregion

    #region Properties

    public int ChunkSize => gameController.GameVariables.chunkSize;

    public GameDatabase GameDatabase => gameController.GameDatabase;

    #endregion

    #region Synchronous Creation

    public void CreateBox(
        Vector2Int firstPosition,
        Vector2Int secondPosition,
        bool render = false)
    {
        if (!ValidateGenerationReferences())
        {
            return;
        }

        Vector2Int start = Vector2Int.Min(
            firstPosition,
            secondPosition
        );

        Vector2Int end = Vector2Int.Max(
            firstPosition,
            secondPosition
        );

        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                GetOrCreateChunk(
                    new Vector2Int(x, y),
                    render
                );
            }
        }
    }

    public void CreateBox(
        Vector2Int center,
        int radius,
        bool render = false)
    {
        Vector2Int radiusOffset =
            new(radius, radius);

        CreateBox(
            center - radiusOffset,
            center + radiusOffset,
            render
        );
    }

    public Chunk GetOrCreateChunk(
        Vector2Int chunkPosition,
        bool render = false)
    {
        if (!ValidateGenerationReferences())
        {
            return null;
        }

        Chunk chunk = GetOrAddChunk(chunkPosition);

        if (chunk.State == ChunkState.Ungenerated)
        {
            chunk = GenerateChunk(
                chunkPosition,
                chunk
            );
        }

        if (render &&
            chunk.State == ChunkState.Generated)
        {
            RenderChunk(chunkPosition);
        }

        return chunk;
    }

    #endregion

    #region Asynchronous Creation

    public void AsyncCreateBox(
        Vector2Int firstPosition,
        Vector2Int secondPosition,
        bool render = true)
    {
        if (!ValidateGenerationReferences())
        {
            return;
        }

        Vector2Int start = Vector2Int.Min(
            firstPosition,
            secondPosition
        );

        Vector2Int end = Vector2Int.Max(
            firstPosition,
            secondPosition
        );

        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                QueueChunk(
                    new Vector2Int(x, y),
                    render
                );
            }
        }

        StartLoadingCoroutine();
    }

    public void AsyncCreateBox(
        Vector2Int center,
        int radius,
        bool render = true)
    {
        Vector2Int radiusOffset =
            new(radius, radius);

        AsyncCreateBox(
            center - radiusOffset,
            center + radiusOffset,
            render
        );
    }

    public void AsyncCreateRadius(
        Vector2Int center,
        int radius,
        bool render = true)
    {
        if (!ValidateGenerationReferences())
        {
            return;
        }

        int radiusSquared = radius * radius;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int offset = new(x, y);

                if (offset.sqrMagnitude > radiusSquared)
                {
                    continue;
                }

                QueueChunk(
                    center + offset,
                    render
                );
            }
        }

        StartLoadingCoroutine();
    }

    public void QueueChunkForLoading(
        Vector2Int chunkPosition,
        bool render = true)
    {
        if (!ValidateGenerationReferences())
        {
            return;
        }

        QueueChunk(
            chunkPosition,
            render
        );

        StartLoadingCoroutine();
    }

    #endregion

    #region Queue Management

    private void QueueChunk(
        Vector2Int chunkPosition,
        bool render)
    {
        Chunk chunk = GetOrAddChunk(chunkPosition);

        if (chunk.State == ChunkState.Rendered)
        {
            return;
        }

        if (queuedChunkSet.Add(chunkPosition))
        {
            queuedChunks.Enqueue(chunkPosition);
            renderRequests[chunkPosition] = render;

            return;
        }

        // Upgrade an existing request from generation-only
        // to generation + render.
        if (render)
        {
            renderRequests[chunkPosition] = true;
        }
    }

    private IEnumerator ProcessChunkQueue()
    {
        while (queuedChunks.Count > 0)
        {
            Vector2Int chunkPosition =
                queuedChunks.Dequeue();

            // A cancelled/deleted request remains physically in Queue<T>,
            // but is ignored because it is no longer in the active set.
            if (!queuedChunkSet.Remove(chunkPosition))
            {
                continue;
            }

            bool shouldRender =
                renderRequests.TryGetValue(
                    chunkPosition,
                    out bool requestedRender
                ) &&
                requestedRender;

            renderRequests.Remove(chunkPosition);

            if (!chunks.TryGetValue(
                    chunkPosition,
                    out Chunk chunk))
            {
                yield return null;
                continue;
            }

            if (chunk.State == ChunkState.Ungenerated)
            {
                chunk = GenerateChunk(
                    chunkPosition,
                    chunk
                );

                yield return null;
            }

            if (shouldRender &&
                chunk.State == ChunkState.Generated)
            {
                RenderChunk(chunkPosition);

                yield return null;
            }
        }

        loadingCoroutine = null;
    }

    private void CancelQueuedChunk(
        Vector2Int chunkPosition)
    {
        queuedChunkSet.Remove(chunkPosition);
        renderRequests.Remove(chunkPosition);
    }

    private void StartLoadingCoroutine()
    {
        if (loadingCoroutine != null)
        {
            return;
        }

        loadingCoroutine =
            StartCoroutine(ProcessChunkQueue());
    }

    private void StopLoadingCoroutine()
    {
        if (loadingCoroutine == null)
        {
            return;
        }

        StopCoroutine(loadingCoroutine);

        loadingCoroutine = null;
    }

    #endregion

    #region Generation

    private Chunk GetOrAddChunk(
        Vector2Int chunkPosition)
    {
        if (chunks.TryGetValue(
                chunkPosition,
                out Chunk chunk))
        {
            return chunk;
        }

        chunk = new Chunk(
            ChunkSize,
            ChunkState.Ungenerated
        );

        chunks.Add(
            chunkPosition,
            chunk
        );

        return chunk;
    }

    private Chunk GenerateChunk(
        Vector2Int chunkPosition,
        Chunk chunk)
    {
        Chunk generatedChunk =
            generator.GenerateChunk(
                chunkPosition,
                chunk
            );

        if (generatedChunk == null)
        {
            Debug.LogError(
                $"Generator returned null for chunk {chunkPosition}.",
                this
            );

            chunk.State = ChunkState.Ungenerated;

            return chunk;
        }

        generatedChunk.State =
            ChunkState.Generated;

        if (!ReferenceEquals(
                generatedChunk,
                chunk))
        {
            chunks[chunkPosition] =
                generatedChunk;
        }

        return generatedChunk;
    }

    #endregion

    #region Unloading / Deletion

    public void UnloadChunk(
        Vector2Int chunkPosition)
    {
        if (!chunks.TryGetValue(
                chunkPosition,
                out Chunk chunk))
        {
            return;
        }

        CancelQueuedChunk(chunkPosition);

        if (chunk.State == ChunkState.Rendered)
        {
            UnRenderChunk(chunkPosition);
        }
    }

    public void UnloadAllChunks()
    {
        List<Vector2Int> positions =
            new(chunks.Keys);

        foreach (Vector2Int position in positions)
        {
            UnloadChunk(position);
        }
    }

    public void DeleteChunk(
        Vector2Int chunkPosition)
    {
        if (!chunks.TryGetValue(
                chunkPosition,
                out Chunk chunk))
        {
            return;
        }

        CancelQueuedChunk(chunkPosition);

        if (chunk.State == ChunkState.Rendered)
        {
            UnRenderChunk(chunkPosition);
        }

        chunks.Remove(chunkPosition);
    }

    public void DeleteAllChunks()
    {
        StopLoadingCoroutine();

        queuedChunks.Clear();
        queuedChunkSet.Clear();
        renderRequests.Clear();

        chunks.Clear();
        tileCache.Clear();

        if (walls != null)
        {
            walls.ClearAllTiles();
        }

        if (floors != null)
        {
            floors.ClearAllTiles();
        }
    }

    #endregion

    #region Chunk Queries

    public bool HasChunkAtWorldLocation(
        Vector3 position)
    {
        Vector2Int chunkPosition =
            ChunkUtilities.WorldToChunkCoord(
                position,
                ChunkSize
            );

        return chunks.ContainsKey(chunkPosition);
    }

    public bool HasChunkAtBlockLocation(
        Vector2Int blockPosition)
    {
        Vector2Int chunkPosition =
            ChunkUtilities.WorldToChunkCoord(
                (Vector2)blockPosition,
                ChunkSize
            );

        return chunks.ContainsKey(chunkPosition);
    }

    public bool IsChunkGenerated(
        Vector2Int chunkPosition)
    {
        if (!chunks.TryGetValue(
                chunkPosition,
                out Chunk chunk))
        {
            return false;
        }

        return chunk.State == ChunkState.Generated ||
               chunk.State == ChunkState.Rendered;
    }

    public bool IsChunkRendered(
        Vector2Int chunkPosition)
    {
        return chunks.TryGetValue(
                   chunkPosition,
                   out Chunk chunk
               ) &&
               chunk.State == ChunkState.Rendered;
    }

    public bool IsChunkQueued(
        Vector2Int chunkPosition)
    {
        return queuedChunkSet.Contains(chunkPosition);
    }

    #endregion

    #region Tile Queries

    public int GetWallIDAtLocation(
        Vector2 position)
    {
        if (!TryGetChunkAndLocalPosition(
                position,
                out Chunk chunk,
                out Vector2Int localPosition))
        {
            return -1;
        }

        return chunk.GetWallTile(localPosition);
    }

    public int GetFloorIDAtLocation(
        Vector2 position)
    {
        if (!TryGetChunkAndLocalPosition(
                position,
                out Chunk chunk,
                out Vector2Int localPosition))
        {
            return -1;
        }

        return chunk.GetFloorTile(localPosition);
    }

    public TileDataAsset GetWallDataAtLocation(
        Vector2 position)
    {
        return GetTileData(
            GetWallIDAtLocation(position)
        );
    }

    public TileDataAsset GetFloorDataAtLocation(
        Vector2 position)
    {
        return GetTileData(
            GetFloorIDAtLocation(position)
        );
    }

    private TileDataAsset GetTileData(int id)
    {
        if (id < 0 ||
            GameDatabase == null)
        {
            return null;
        }

        if (tileCache.TryGetValue(
                id,
                out TileDataAsset data))
        {
            return data;
        }

        data =
            GameDatabase.GetAssetByID<TileDataAsset>(id);

        tileCache[id] = data;

        return data;
    }

    #endregion

    #region Rendering

    private void RenderChunk(
        Vector2Int chunkPosition)
    {
        if (!chunks.TryGetValue(
                chunkPosition,
                out Chunk chunk))
        {
            Debug.LogError(
                $"Chunk {chunkPosition} does not exist.",
                this
            );

            return;
        }

        if (chunk.State == ChunkState.Rendered)
        {
            return;
        }

        if (chunk.State != ChunkState.Generated)
        {
            Debug.LogError(
                $"Chunk {chunkPosition} cannot be rendered " +
                $"from state {chunk.State}.",
                this
            );

            return;
        }

        if (!HasValidRenderingReferences())
        {
            return;
        }

        int cellCount =
            ChunkSize * ChunkSize;

        Vector3Int[] positions =
            new Vector3Int[cellCount];

        TileBase[] wallTiles =
            new TileBase[cellCount];

        TileBase[] floorTiles =
            new TileBase[cellCount];

        int index = 0;

        for (int y = 0; y < ChunkSize; y++)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                Vector2Int localPosition =
                    new(x, y);

                Vector2Int worldPosition =
                    ChunkUtilities.LocalToWorldCoord(
                        localPosition,
                        chunkPosition,
                        ChunkSize
                    );

                int wallTileID =
                    chunk.GetWallTile(localPosition);

                int floorTileID =
                    chunk.GetFloorTile(localPosition);

                TileDataAsset wallData =
                    GetTileData(wallTileID);

                TileDataAsset floorData =
                    GetTileData(floorTileID);

                positions[index] =
                    new Vector3Int(
                        worldPosition.x,
                        worldPosition.y,
                        0
                    );

                if (wallData != null)
                {
                    wallTiles[index] =
                        wallData.Tile;

                    if (wallData.IsTransparent &&
                        floorData != null)
                    {
                        floorTiles[index] =
                            floorData.Tile;
                    }
                }
                else if (floorData != null)
                {
                    floorTiles[index] =
                        floorData.Tile;
                }

                index++;
            }
        }

        walls.SetTiles(
            positions,
            wallTiles
        );

        floors.SetTiles(
            positions,
            floorTiles
        );

        chunk.State =
            ChunkState.Rendered;
    }

    private void UnRenderChunk(
        Vector2Int chunkPosition)
    {
        if (!chunks.TryGetValue(
                chunkPosition,
                out Chunk chunk))
        {
            return;
        }

        if (chunk.State != ChunkState.Rendered)
        {
            return;
        }

        if (!HasValidTilemaps())
        {
            return;
        }

        int cellCount =
            ChunkSize * ChunkSize;

        Vector3Int[] positions =
            new Vector3Int[cellCount];

        TileBase[] emptyTiles =
            new TileBase[cellCount];

        int index = 0;

        for (int y = 0; y < ChunkSize; y++)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                Vector2Int worldPosition =
                    ChunkUtilities.LocalToWorldCoord(
                        new Vector2Int(x, y),
                        chunkPosition,
                        ChunkSize
                    );

                positions[index] =
                    new Vector3Int(
                        worldPosition.x,
                        worldPosition.y,
                        0
                    );

                index++;
            }
        }

        walls.SetTiles(
            positions,
            emptyTiles
        );

        floors.SetTiles(
            positions,
            emptyTiles
        );

        chunk.State =
            ChunkState.Generated;
    }

    #endregion

    #region Position Helpers

    private bool TryGetChunkAndLocalPosition(
        Vector2 position,
        out Chunk chunk,
        out Vector2Int localPosition)
    {
        Vector2Int chunkPosition =
            ChunkUtilities.WorldToChunkCoord(
                position,
                ChunkSize
            );

        localPosition =
            ChunkUtilities.WorldToLocalCoord(
                position,
                ChunkSize
            );

        if (!chunks.TryGetValue(
                chunkPosition,
                out chunk))
        {
            Debug.LogWarning(
                $"No chunk exists at {chunkPosition}.",
                this
            );

            return false;
        }

        if (chunk.State == ChunkState.Ungenerated)
        {
            Debug.LogWarning(
                $"Chunk {chunkPosition} is not generated.",
                this
            );

            return false;
        }

        return true;
    }

    #endregion

    #region Validation

    private bool ValidateGenerationReferences()
    {
        if (gameController == null)
        {
            Debug.LogError(
                "ChunkManager is missing a GameController.",
                this
            );

            return false;
        }

        if (generator == null)
        {
            Debug.LogError(
                "ChunkManager is missing a WorldGenerator.",
                this
            );

            return false;
        }

        return true;
    }

    private bool HasValidRenderingReferences()
    {
        if (!HasValidTilemaps())
        {
            return false;
        }

        if (GameDatabase == null)
        {
            Debug.LogError(
                "ChunkManager is missing a GameDatabase.",
                this
            );

            return false;
        }

        return true;
    }

    private bool HasValidTilemaps()
    {
        if (walls != null &&
            floors != null)
        {
            return true;
        }

        Debug.LogError(
            "ChunkManager is missing wall or floor Tilemaps.",
            this
        );

        return false;
    }

    #endregion
}