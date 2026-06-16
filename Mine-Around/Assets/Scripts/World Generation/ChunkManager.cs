using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

    [Header("Chunk Settings")]
    [SerializeField] private int chunkSize = 16;
    [SerializeField] private int maxChunkRange = 4;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap walls;
    [SerializeField] private Tilemap floors;

    [Header("Generation")]
    [SerializeField] private WorldGenerator generator;

    private readonly Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    public int ChunkSize => chunkSize;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CreateRadiusAt(Vector2Int chunkLocation)
    {
        for (int x = -maxChunkRange; x <= maxChunkRange; x++)
        {
            for (int y = -maxChunkRange; y <= maxChunkRange; y++)
            {
                Vector2Int offset = new Vector2Int(x, y);
                LoadChunk(chunkLocation + offset);
            }
        }
    }

    public void CreateChunk(Vector2Int chunkLocation)
    {
        GetOrCreateChunk(chunkLocation);
    }

    private Chunk GetOrCreateChunk(Vector2Int chunkLocation)
    {
        if (chunks.TryGetValue(chunkLocation, out Chunk chunk))
            return chunk;

        if (generator == null)
        {
            Debug.LogError("ChunkManager is missing a WorldGenerator reference.", this);
            return null;
        }

        chunk = generator.GenerateChunk(chunkLocation, chunkSize);
        chunks.Add(chunkLocation, chunk);

        return chunk;
    }

    public void LoadChunk(Vector2Int chunkLocation)
    {
        Chunk chunk = GetOrCreateChunk(chunkLocation);

        if (chunk == null)
            return;

        if (chunk.loaded)
            return;

        chunk.loaded = true;

        RenderChunk(chunkLocation, chunk);
    }

    private void RenderChunk(Vector2Int chunkLocation, Chunk chunk)
    {
        Vector3Int[] positions = new Vector3Int[chunkSize * chunkSize];
        TileBase[] wallTiles = new TileBase[chunkSize * chunkSize];
        TileBase[] floorTiles = new TileBase[chunkSize * chunkSize];

        int i = 0;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = ChunkUtilities.LocalToWorldCoord(localPos, chunkLocation, chunkSize);

                string wallTileData = chunk.GetWorldWallTile(localPos);
                string floorTileData = chunk.GetWorldFloorTile(localPos);

                //string wallID = wallTileData != null ? wallTileData : string.Empty;
                //string floorID = floorTileData != null ? floorTileData : string.Empty;

                TileData wallData = WorldDataObjectDataBase.Instance.GetAssetByID<TileData>(wallTileData);
                TileData floorData = WorldDataObjectDataBase.Instance.GetAssetByID<TileData>(floorTileData);

                bool isTransparent = wallData.transparent;

                positions[i] = (Vector3Int)worldPos;
                wallTiles[i] = wallData != null ? wallData.tile : null;
                floorTiles[i] = isTransparent && floorData != null ? floorData.tile : null;

                i++;
            }
        }

        walls.SetTiles(positions, wallTiles);
        floors.SetTiles(positions, floorTiles);
    }

    public void UnloadChunk(Vector2Int chunkLocation)
    {
        if (!chunks.TryGetValue(chunkLocation, out Chunk chunk))
            return;

        if (!chunk.loaded)
            return;

        chunk.loaded = false;

        ClearChunkTiles(chunkLocation);
    }

    private void ClearChunkTiles(Vector2Int chunkLocation)
    {
        Vector3Int[] positions = new Vector3Int[chunkSize * chunkSize];
        TileBase[] wallTiles = new TileBase[chunkSize * chunkSize];
        TileBase[] floorTiles = new TileBase[chunkSize * chunkSize];

        int i = 0;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = ChunkUtilities.LocalToWorldCoord(localPos, chunkLocation, chunkSize);

                positions[i] = (Vector3Int)worldPos;
                wallTiles[i] = null;
                floorTiles[i] = null;

                i++;
            }
        }

        walls.SetTiles(positions, wallTiles);
        floors.SetTiles(positions, floorTiles);
    }

    public string GetWorldWallTileAtLocation(Vector2 location)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(location, chunkSize);
        Vector2Int localPos = ChunkUtilities.WorldToLocalCoord(location, chunkSize);

        if (!chunks.TryGetValue(chunkCoord, out Chunk chunk))
        {
            Debug.LogWarning($"No chunk exists at chunk coordinate {chunkCoord}.", this);
            return null;
        }

        return chunk.GetWorldWallTile(localPos);
    }

    public string GetWorldFloorTileAtLocation(Vector2 location)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(location, chunkSize);
        Vector2Int localPos = ChunkUtilities.WorldToLocalCoord(location, chunkSize);

        if (!chunks.TryGetValue(chunkCoord, out Chunk chunk))
        {
            Debug.LogWarning($"No chunk exists at chunk coordinate {chunkCoord}.", this);
            return null;
        }

        return chunk.GetWorldFloorTile(localPos);
    }

    public void UnloadAllChunks()
    {
        List<Vector2Int> chunkLocations = new List<Vector2Int>(chunks.Keys);

        foreach (Vector2Int chunkLocation in chunkLocations)
        {
            UnloadChunk(chunkLocation);
        }
    }

    public void DeleteAllChunks()
    {
        walls.ClearAllTiles();
        floors.ClearAllTiles();
        chunks.Clear();
    }

    public bool InWorldBounds(Vector3 location)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(location, chunkSize);
        return chunks.ContainsKey(chunkCoord);
    }

    public bool InWorldBounds(Vector2Int blockLocation)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord((Vector3Int)blockLocation, chunkSize);
        return chunks.ContainsKey(chunkCoord);
    }

    public bool HasChunk(Vector2Int chunkLocation)
    {
        return chunks.ContainsKey(chunkLocation);
    }
}