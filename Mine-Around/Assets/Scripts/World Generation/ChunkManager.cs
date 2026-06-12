using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkManager : MonoBehaviour
{
    public int chunkSize = 16;
    [SerializeField] private int maxChunkRange = 4;
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    [SerializeField] private Tilemap walls;
    [SerializeField] private Tilemap floors;

    [SerializeField] private WorldGenerator generator;

    public static ChunkManager Instance { get; private set; }

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
        for(int x = -maxChunkRange; x < maxChunkRange; x++)
        {
            for(int y = -maxChunkRange; y < maxChunkRange; y++)
            {
                Vector2Int loadingPos = new Vector2Int(x, y);
                LoadChunk(chunkLocation + loadingPos);
            }
        }
    }

    public void CreateChunk(Vector2Int chunkLocation)
    {
        if (chunks.ContainsKey(chunkLocation)) return;

        Chunk chunk = generator.GenerateChunk(chunkLocation, chunkSize);
        chunks.Add(chunkLocation, chunk);
    }

    public void LoadChunk(Vector2Int chunkLocation)
    {
        if (!chunks.ContainsKey(chunkLocation)) CreateChunk(chunkLocation);

        Chunk chunk = chunks[chunkLocation];

        chunk.loaded = true;

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

                string worldWall = chunk.GetWorldWallTile(localPos).nameID;
                string worldFloor = chunk.GetWorldFloorTile(localPos).nameID;

                Tile nextWallTile = WorldDataRegistry.Instance.GetTileData(worldWall).tile;
                Tile nextFloorTile = WorldDataRegistry.Instance.GetTileData(worldFloor).tile;

                positions[i] = (Vector3Int)worldPos;
                wallTiles[i] = nextWallTile;
                floorTiles[i] = nextFloorTile;
                i++;
            }
        }

        walls.SetTiles(positions, wallTiles);
        floors.SetTiles(positions, floorTiles);
    }

    public void UnloadChunk(Vector2Int chunkLocation)
    {
        if (!chunks.ContainsKey(chunkLocation))
            return;

        Chunk chunk = chunks[chunkLocation];

        if (!chunk.loaded) return;

        chunk.loaded = false;

        Vector3Int[] positions = new Vector3Int[chunkSize * chunkSize];
        TileBase[] wallTiles = new TileBase[chunkSize * chunkSize];
        TileBase[] floorTiles = new TileBase[chunkSize * chunkSize];

        int i = 0;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int worldPos = ChunkUtilities.LocalToWorldCoord(new Vector2Int(x, y), chunkLocation, chunkSize);

                positions[i] = (Vector3Int)worldPos;
                wallTiles[i] = null;
                floorTiles[i] = null;
                i++;
            }
        }

        walls.SetTiles(positions, wallTiles);
        floors.SetTiles(positions, floorTiles);
    }

    public WorldTile GetWorldWallTileAtLocation(Vector2 location)
    {

        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(location, chunkSize);
        Vector2Int localPos = ChunkUtilities.WorldToLocalCoord(location, chunkSize);

        return chunks[chunkCoord].GetWorldWallTile(localPos);
    }

    public WorldTile GetWorldFloorTileAtLocation(Vector2 location)
    {

        Vector2Int chunkCoord = ChunkUtilities.WorldToChunkCoord(location, chunkSize);
        Vector2Int localPos = ChunkUtilities.WorldToLocalCoord(location, chunkSize);

        return chunks[chunkCoord].GetWorldFloorTile(localPos);
    }

    public void UnloadAllChunks()
    {
        foreach (Vector2Int chunkLocation in chunks.Keys)
        {
            UnloadChunk(chunkLocation);
        }
    }

    public void DeleteAllChunks()
    {
        UnloadAllChunks();
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
}
