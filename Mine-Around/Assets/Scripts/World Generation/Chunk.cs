using UnityEngine;

public class Chunk
{
    public readonly int chunkSize;
    public WorldWallTile[,] WorldWallTiles;
    public WorldFloorTile[,] WorldFloorTiles;
    public bool loaded;

    public Chunk(int size)
    {
        chunkSize = size;

        WorldWallTiles = new WorldWallTile[size, size];
        WorldFloorTiles = new WorldFloorTile[size, size];

        loaded = false;
    }

    public bool InBounds(Vector2Int tileLocation)
    {
        return tileLocation.x >= 0 && tileLocation.x < chunkSize &&
               tileLocation.y >= 0 && tileLocation.y < chunkSize;
    }

    public WorldWallTile GetWorldWallTile(Vector2Int tileLocation)
    {
        if(!InBounds(tileLocation)) Debug.LogError("Not in bounds");

        return WorldWallTiles[tileLocation.x, tileLocation.y];
    }

    public void SetWorldWallTile(Vector2Int tileLocation, WorldWallTile tile)
    {
        if(!InBounds(tileLocation)) Debug.LogError("Not in bounds");

        WorldWallTiles[tileLocation.x, tileLocation.y] = tile;
    }

    public WorldFloorTile GetWorldFloorTile(Vector2Int tileLocation)
    {
        if(!InBounds(tileLocation)) Debug.LogError("Not in bounds");

        return WorldFloorTiles[tileLocation.x, tileLocation.y];
    }

    public void SetWorldFloorTile(Vector2Int tileLocation, WorldFloorTile tile)
    {
        if(!InBounds(tileLocation)) Debug.LogError("Not in bounds");

        WorldFloorTiles[tileLocation.x, tileLocation.y] = tile;
    }

}