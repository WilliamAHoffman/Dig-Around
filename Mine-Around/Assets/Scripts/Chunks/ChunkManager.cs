using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Manages chunk generation, loading, rendering, unloading, and tile lookup
// Chunk coordinates are grid positions in chunk space, not world tile positions
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

    // Global chunk values
    public int ChunkSize => chunkSize;
    public int MaxChunkRange => maxChunkRange;

    // Sets up self static instance

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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

                if (render)
                {
                    LoadChunk(chunkLocation);
                }
                else
                {
                    GetOrCreateChunk(chunkLocation);
                }
            }
        }
    }

    // Gets and or creates a chunk at a chunkPosition
    // Does not render the chunk
    // Returns the created chunk
    private Chunk GetOrCreateChunk(Vector2Int chunkPosition)
    {
        if (chunks.TryGetValue(chunkPosition, out Chunk chunk))
            return chunk;

        if (generator == null)
        {
            Debug.LogError("ChunkManager is missing a WorldGenerator reference.", this);
            return null;
        }

        chunk = generator.GenerateChunk(chunkPosition, chunkSize);

        if (chunk == null)
        {
            Debug.LogError($"WorldGenerator failed to generate chunk at {chunkPosition}.", this);
            return null;
        }

        chunks.Add(chunkPosition, chunk);

        return chunk;
    }

    // Creates a chunk if needed, marks it loaded, renders the chunk and then returns it
    public Chunk LoadChunk(Vector2Int chunkPosition)
    {
        Chunk chunk = GetOrCreateChunk(chunkPosition);

        if (chunk == null)
            return null;

        if (chunk.loaded)
            return chunk;

        chunk.loaded = true;
        RenderChunk(chunkPosition);

        return chunk;
    }

    // Marks chunk as unloaded and derenders it
    public void UnloadChunk(Vector2Int chunkLocation)
    {
        if (!chunks.TryGetValue(chunkLocation, out Chunk chunk))
            return;

        if (!chunk.loaded)
            return;

        chunk.loaded = false;
        UnRenderChunk(chunkLocation);
    }

    // Places the tiles from a chunk at the correct position on the tilemaps
    private void RenderChunk(Vector2Int chunkLocation)
    {
        Chunk chunk = chunks[chunkLocation];

        if (chunk == null)
        {
            Debug.LogError("No valid chunk at the given location.", this);
            return;
        }

        if (!HasValidTilemaps())
        {
            Debug.LogError("Tilemaps are missing.", this);
            return;
        }

        if (WorldDataObjectDataBase.Instance == null)
        {
            Debug.LogError("WorldDataObjectDataBase instance is missing.", this);
            return;
        }

        Vector3Int[] positions = new Vector3Int[chunkSize * chunkSize];
        TileBase[] wallTiles = new TileBase[chunkSize * chunkSize];
        TileBase[] floorTiles = new TileBase[chunkSize * chunkSize];

        int i = 0;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = ChunkUtilities.LocalToWorldCoord(localPos, chunkLocation);

                string wallTileID = chunk.GetWallTileID(localPos);
                string floorTileID = chunk.GetFloorTileID(localPos);

                TileData wallData = WorldDataObjectDataBase.Instance.GetAssetByID<TileData>(wallTileID);
                TileData floorData = WorldDataObjectDataBase.Instance.GetAssetByID<TileData>(floorTileID);

                bool showFloor = wallData == null || wallData.transparent;

                positions[i] = (Vector3Int)worldPos;
                wallTiles[i] = wallData != null ? wallData.tile : null;
                floorTiles[i] = showFloor && floorData != null ? floorData.tile : null;

                i++;
            }
        }

        walls.SetTiles(positions, wallTiles);
        floors.SetTiles(positions, floorTiles);
    }

    // Removes tiles from tilemaps of the chunk at the given position
    private void UnRenderChunk(Vector2Int chunkPosition)
    {
        if (!HasValidTilemaps())
            return;

        Vector3Int[] positions = new Vector3Int[chunkSize * chunkSize];
        TileBase[] emptyTiles = new TileBase[chunkSize * chunkSize];

        int i = 0;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = ChunkUtilities.LocalToWorldCoord(localPos, chunkPosition);

                positions[i] = (Vector3Int)worldPos;
                emptyTiles[i] = null;

                i++;
            }
        }

        walls.SetTiles(positions, emptyTiles);
        floors.SetTiles(positions, emptyTiles);
    }

    // Gives the wall string ID at the given location if it exists
    public string GetWallIDAtLocation(Vector2 position)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(position);
        Vector2Int localPos = ChunkUtilities.WorldToLocalCoord(position);

        if (!chunks.TryGetValue(chunkCoord, out Chunk chunk))
        {
            Debug.LogWarning($"No chunk exists at chunk coordinate {chunkCoord}.", this);
            return null;
        }

        return chunk.GetWallTileID(localPos);
    }
    

    // Gives the floor string ID at the given location if it exists
    public string GetFloorIDAtLocation(Vector2 position)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(position);
        Vector2Int localPos = ChunkUtilities.WorldToLocalCoord(position);

        if (!chunks.TryGetValue(chunkCoord, out Chunk chunk))
        {
            Debug.LogWarning($"No chunk exists at chunk coordinate {chunkCoord}.", this);
            return null;
        }

        return chunk.GetFloorTileID(localPos);
    }

    // Gives the wall data asset at the given location if it exists
    public TileData GetWallDataAtLocation(Vector2 location)
    {
        if (WorldDataObjectDataBase.Instance == null)
        {
            Debug.LogError("WorldDataObjectDataBase instance is missing.", this);
            return null;
        }

        string tileID = GetWallIDAtLocation(location);

        if (string.IsNullOrEmpty(tileID))
            return null;

        return WorldDataObjectDataBase.Instance.GetTileData(tileID);
    }

    // Gives the floor data asset at the given location if it exists
    public TileData GetFloorDataAtLocation(Vector2 location)
    {
        if (WorldDataObjectDataBase.Instance == null)
        {
            Debug.LogError("WorldDataObjectDataBase instance is missing.", this);
            return null;
        }

        string tileID = GetFloorIDAtLocation(location);

        if (string.IsNullOrEmpty(tileID))
            return null;

        return WorldDataObjectDataBase.Instance.GetTileData(tileID);
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

        chunks.Clear();
    }

    // Checks if a chunk exists at a world position
    public bool HasChunkAtWorldLocation(Vector3 position)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(position);
        return chunks.ContainsKey(chunkCoord);
    }

    // Checks if a chunk exists at a block position
    public bool HasChunkAtBlockLocation(Vector2Int blockPosition)
    {
        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord((Vector3Int)blockPosition);
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
}

