using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System;

// Manages chunk generation, loading, rendering, unloading, and tile lookup
// Chunk coordinates are grid positions in chunk space, not world tile positions
public class ChunkManager : MonoBehaviour
{

    [Header("Tilemaps")]
    [SerializeField] private Tilemap walls;
    [SerializeField] private Tilemap floors;

    [Header("Generation")]
    [SerializeField] private WorldGenerator generator;

    private readonly Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    // Global chunk values
    [SerializeField] private GameController gameController;
    public int ChunkSize => gameController.GameVariables.chunkSize;
    public GameDatabase GameDatabase => gameController.GameDatabase;

    // Chunk Queueing
    private readonly Queue<Vector2Int> queuedChunks = new();
    private readonly HashSet<Vector2Int> queuedChunkSet = new();
    private Coroutine loadingCoroutine;

    // Loads or Creates all chunks within the two given positions
    // Can be set to render automatically
    public void CreateBox(Vector2Int chunkPosition1, Vector2Int chunkPosition2, bool render = false)
    {
        Vector2Int start = Vector2Int.Min(chunkPosition1, chunkPosition2);
        Vector2Int end = Vector2Int.Max(chunkPosition1, chunkPosition2);

        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                Vector2Int chunkLocation = new Vector2Int(x, y);

                GetOrCreateChunk(chunkLocation, render);
            }
        }
    }

    public void CreateBox(Vector2Int chunkPosition, int radius, bool render = false)
    {
        Vector2Int start = new Vector2Int(chunkPosition.x - radius, chunkPosition.y - radius);
        Vector2Int end = new Vector2Int(chunkPosition.x + radius, chunkPosition.y + radius);
        CreateBox(start, end, render);
    }


    // Creates a chunk if needed, marks it loaded, renders the chunk if needed and then returns it
    public Chunk GetOrCreateChunk(Vector2Int chunkPosition, bool render = false)
    {

        if (generator == null)
        {
            Debug.LogError("ChunkManager is missing a WorldGenerator reference.", this);
            return null;
        }

        Chunk chunk = null;

        if (chunks.TryGetValue(chunkPosition, out Chunk currChunk))
        {
            if (currChunk.state == ChunkState.queued || currChunk.state == ChunkState.dequeue)
            {
                chunk = generator.GenerateChunk(chunkPosition, currChunk);
                chunk.state = ChunkState.saved;
            }
        }
        else
        {
            chunk = generator.GenerateChunk(chunkPosition, new Chunk(ChunkSize));
            chunks.Add(chunkPosition, chunk);
        }

        if (render)
        {
            RenderChunk(chunkPosition);
        }

        return chunk;
    }

    // Marks chunk as unloaded and derenders it
    public void UnloadChunk(Vector2Int chunkLocation)
    {
        if (!chunks.TryGetValue(chunkLocation, out Chunk chunk))
            return;

        if (chunk.state == ChunkState.queued)
        {
            chunk.state = ChunkState.dequeue;
        }

        if (chunk.state == ChunkState.saved)
        {
            return;
        }

        UnRenderChunk(chunkLocation);
    }

    // Places the tiles from a chunk at the correct position on the tilemaps
    private void RenderChunk(Vector2Int chunkLocation)
    {
        Chunk chunk = chunks[chunkLocation];

        if (chunk == null || chunk.state != ChunkState.saved)
        {
            Debug.LogError("No valid chunk at the given location.", this);
            return;
        }

        if (!HasValidTilemaps())
        {
            Debug.LogError("Tilemaps are missing.", this);
            return;
        }

        if (GameDatabase == null)
        {
            Debug.LogError("GameDatabase instance is missing.", this);
            return;
        }

        Vector3Int[] positions = new Vector3Int[ChunkSize * ChunkSize];
        TileBase[] wallTiles = new TileBase[ChunkSize * ChunkSize];
        TileBase[] floorTiles = new TileBase[ChunkSize * ChunkSize];

        int i = 0;

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = ChunkUtilities.LocalToWorldCoord(localPos, chunkLocation, ChunkSize);

                int wallTileID = chunk.GetWallTileID(localPos);
                int floorTileID = chunk.GetFloorTileID(localPos);

                TileData wallData = GameDatabase.GetAssetByID<TileData>(wallTileID);
                TileData floorData = GameDatabase.GetAssetByID<TileData>(floorTileID);

                bool showFloor = wallData == null || wallData.transparent;

                positions[i] = (Vector3Int)worldPos;
                wallTiles[i] = wallData != null ? wallData.tile : null;
                floorTiles[i] = showFloor && floorData != null ? floorData.tile : null;

                i++;
            }
        }

        walls.SetTiles(positions, wallTiles);
        floors.SetTiles(positions, floorTiles);
        chunk.state = ChunkState.rendered;
    }

    // Removes tiles from tilemaps of the chunk at the given position
    private void UnRenderChunk(Vector2Int chunkPosition)
    {
        if (!HasValidTilemaps())
            return;

        Vector3Int[] positions = new Vector3Int[ChunkSize * ChunkSize];
        TileBase[] emptyTiles = new TileBase[ChunkSize * ChunkSize];

        int i = 0;

        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = ChunkUtilities.LocalToWorldCoord(localPos, chunkPosition, ChunkSize);

                positions[i] = (Vector3Int)worldPos;
                emptyTiles[i] = null;

                i++;
            }
        }

        walls.SetTiles(positions, emptyTiles);
        floors.SetTiles(positions, emptyTiles);

        if (chunks.ContainsKey(chunkPosition))
        {
            if (chunks[chunkPosition].state == ChunkState.queued)
            {
                chunks[chunkPosition].state = ChunkState.dequeue;
            }
            else
            {
                chunks[chunkPosition].state = ChunkState.saved;
            }
        }
    }

    // Gives the wall string ID at the given location if it exists
    public int GetWallIDAtLocation(Vector2 position)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(position, ChunkSize);
        Vector2Int localPos = ChunkUtilities.WorldToLocalCoord(position, ChunkSize);

        if (!chunks.TryGetValue(chunkCoord, out Chunk chunk))
        {
            Debug.LogWarning($"No chunk exists at chunk coordinate {chunkCoord}.", this);
            return -1;
        }

        return chunk.GetWallTileID(localPos);
    }

    // Gives the floor string ID at the given location if it exists
    public int GetFloorIDAtLocation(Vector2 position)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(position, ChunkSize);
        Vector2Int localPos = ChunkUtilities.WorldToLocalCoord(position, ChunkSize);

        if (!chunks.TryGetValue(chunkCoord, out Chunk chunk))
        {
            Debug.LogWarning($"No chunk exists at chunk coordinate {chunkCoord}.", this);
            return -1;
        }

        return chunk.GetFloorTileID(localPos);
    }

    // Gives the wall data asset at the given location if it exists
    public TileData GetWallDataAtLocation(Vector2 location)
    {
        int tileID = GetWallIDAtLocation(location);

        return GameDatabase.GetAssetByID<TileData>(tileID);
    }

    // Gives the floor data asset at the given location if it exists
    public TileData GetFloorDataAtLocation(Vector2 location)
    {

        int tileID = GetFloorIDAtLocation(location);

        return GameDatabase.GetAssetByID<TileData>(tileID);
    }

    // Unloads and UnRenders all chunks
    public void UnloadAllChunks()
    {
        List<Vector2Int> chunkLocations = new List<Vector2Int>(chunks.Keys);

        foreach (Vector2Int chunkLocation in chunkLocations)
        {
            UnloadChunk(chunkLocation);
        }
    }

    // Deletes and unloads all chunks
    public void DeleteAllChunks()
    {

        if (walls != null)
            walls.ClearAllTiles();

        if (floors != null)
            floors.ClearAllTiles();

        queuedChunks.Clear();
        queuedChunkSet.Clear();
        chunks.Clear();
    }

    // Checks if a chunk exists at a world position
    public bool HasChunkAtWorldLocation(Vector3 position)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(position, ChunkSize);
        return chunks.ContainsKey(chunkCoord);
    }

    // Checks if a chunk exists at a block position
    public bool HasChunkAtBlockLocation(Vector2Int blockPosition)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord((Vector3Int)blockPosition, ChunkSize);
        return chunks.ContainsKey(chunkCoord);
    }

    // Checks if tile maps are null
    private bool HasValidTilemaps()
    {
        if (walls == null || floors == null)
        {
            Debug.LogError("ChunkManager is missing wall or floor Tilemap references.", this);
            return false;
        }

        return true;
    }

    private void QueueChunk(Vector2Int coordinate)
    {
        // Already waiting to load.
        if (!queuedChunkSet.Add(coordinate))
        {
            return;
        }

        if (chunks.TryGetValue(coordinate, out Chunk chunk))
        {
            // Already visible.
            if (chunk.state == ChunkState.rendered)
            {
                queuedChunkSet.Remove(coordinate);
                return;
            }

            chunk.state = ChunkState.queued;
        }
        else
        {
            chunks.Add(
                coordinate,
                new Chunk(ChunkSize, ChunkState.queued)
            );
        }

        queuedChunks.Enqueue(coordinate);
    }
    // processes chunk queue
    private IEnumerator ProcessChunkQueue(bool render)
    {
        const double frameBudgetMs = 4.0;

        var stopwatch =
            System.Diagnostics.Stopwatch.StartNew();

        while (queuedChunks.Count > 0)
        {
            stopwatch.Restart();

            while (queuedChunks.Count > 0 &&
                   stopwatch.Elapsed.TotalMilliseconds < frameBudgetMs)
            {
                Vector2Int coordinate = queuedChunks.Dequeue();
                queuedChunkSet.Remove(coordinate);

                if (!chunks.TryGetValue(coordinate, out Chunk chunk))
                {
                    continue;
                }

                // It left the loading area before being processed.
                if (chunk.state == ChunkState.dequeue)
                {
                    chunk.state = ChunkState.saved;
                    continue;
                }

                GetOrCreateChunk(coordinate, render);
            }

            yield return null;
        }

        loadingCoroutine = null;
    }

    public void AsyncCreateBox(Vector2Int chunkPosition1, Vector2Int chunkPosition2, bool render = true)
    {
        Vector2Int start = Vector2Int.Min(
            chunkPosition1,
            chunkPosition2
        );

        Vector2Int end = Vector2Int.Max(
            chunkPosition1,
            chunkPosition2
        );

        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                QueueChunk(new Vector2Int(x, y));
            }
        }

        if (loadingCoroutine == null)
        {
            loadingCoroutine = StartCoroutine(
                ProcessChunkQueue(render)
            );
        }
    }
    public void AsyncCreateBox(Vector2Int chunkPosition, int radius, bool render = false)
    {
        Vector2Int start = new Vector2Int(
            chunkPosition.x - radius,
            chunkPosition.y - radius
        );

        Vector2Int end = new Vector2Int(
            chunkPosition.x + radius,
            chunkPosition.y + radius
        );

        AsyncCreateBox(start, end, render);
    }
}
