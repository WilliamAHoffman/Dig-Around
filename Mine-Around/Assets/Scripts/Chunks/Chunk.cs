using UnityEngine;

public class Chunk
{
    private readonly int chunkSize;
    private int[,] worldWallTiles;
    private int[,] worldFloorTiles;
    public bool loaded;

    public Chunk(int size)
    {
        chunkSize = size;

        worldWallTiles = new int[size, size];
        worldFloorTiles = new int[size, size];

        loaded = false;
    }

    public bool InBounds(Vector2Int tileLocation)
    {
        return tileLocation.x >= 0 && tileLocation.x < chunkSize &&
               tileLocation.y >= 0 && tileLocation.y < chunkSize;
    }

    public int GetWallTileID(Vector2Int tileLocation)
    {
        if (!InBounds(tileLocation))
        {
            Debug.LogError($"Wall tile location out of bounds: {tileLocation}");
            return -1;
        }

        return worldWallTiles[tileLocation.x, tileLocation.y];
    }

    public void SetWorldWallTile(Vector2Int tileLocation, int tile)
    {
        if (!InBounds(tileLocation))
        {
            Debug.LogError($"Wall tile location out of bounds: {tileLocation}");
            return;
        }

        worldWallTiles[tileLocation.x, tileLocation.y] = tile;
    }

    public int GetFloorTileID(Vector2Int tileLocation)
    {
        if (!InBounds(tileLocation))
        {
            Debug.LogError($"Wall tile location out of bounds: {tileLocation}");
            return -1;
        }

        return worldFloorTiles[tileLocation.x, tileLocation.y];
    }

    public void SetWorldFloorTile(Vector2Int tileLocation, int tile)
    {
        if (!InBounds(tileLocation))
        {
            Debug.LogError($"Wall tile location out of bounds: {tileLocation}");
            return;
        }

        worldFloorTiles[tileLocation.x, tileLocation.y] = tile;
    }

    public void SetWorldLocationTile(Vector2Int tileLocation, LocationTiles tile)
    {
        if (!InBounds(tileLocation))
        {
            Debug.LogError($"Tile location out of bounds: {tileLocation}");
            return;
        }

        worldWallTiles[tileLocation.x, tileLocation.y] = tile.wall;
        worldFloorTiles[tileLocation.x, tileLocation.y] = tile.floor;
    }

}