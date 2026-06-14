using UnityEngine;

public class Chunk
{
    public readonly int chunkSize;
    public WorldTile[,] worldWallTiles;
    public WorldTile[,] worldFloorTiles;
    public bool loaded;

    public Chunk(int size)
    {
        chunkSize = size;

        worldWallTiles = new WorldTile[size, size];
        worldFloorTiles = new WorldTile[size, size];

        loaded = false;
    }

    public bool InBounds(Vector2Int tileLocation)
    {
        return tileLocation.x >= 0 && tileLocation.x < chunkSize &&
               tileLocation.y >= 0 && tileLocation.y < chunkSize;
    }

    public WorldTile GetWorldWallTile(Vector2Int tileLocation)
    {
        if (!InBounds(tileLocation))
        {
            Debug.LogError($"Wall tile location out of bounds: {tileLocation}");
            return null;
        }

        return worldWallTiles[tileLocation.x, tileLocation.y];
    }

    public void SetWorldWallTile(Vector2Int tileLocation, WorldTile tile)
    {
        if (!InBounds(tileLocation))
        {
            Debug.LogError($"Wall tile location out of bounds: {tileLocation}");
            return;
        }

        worldWallTiles[tileLocation.x, tileLocation.y] = tile;
    }

    public WorldTile GetWorldFloorTile(Vector2Int tileLocation)
    {
        if (!InBounds(tileLocation))
        {
            Debug.LogError($"Wall tile location out of bounds: {tileLocation}");
            return null;
        }

        return worldFloorTiles[tileLocation.x, tileLocation.y];
    }

    public void SetWorldFloorTile(Vector2Int tileLocation, WorldTile tile)
    {
        if (!InBounds(tileLocation))
        {
            Debug.LogError($"Wall tile location out of bounds: {tileLocation}");
            return;
        }

        worldFloorTiles[tileLocation.x, tileLocation.y] = tile;
    }

}