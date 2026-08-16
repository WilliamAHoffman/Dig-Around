using System;
using UnityEngine;

public enum ChunkState
{
    Ungenerated,
    Generated,
    Rendered
}

public class Chunk
{
    public int ChunkSize { get; }

    public ChunkState State { get; internal set; }

    private readonly int[] wallTiles;
    private readonly int[] floorTiles;

    public Chunk(
        int size,
        ChunkState state = ChunkState.Ungenerated)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                size,
                "Chunk size must be greater than zero."
            );
        }

        ChunkSize = size;
        State = state;

        int cellCount = size * size;

        wallTiles = new int[cellCount];
        floorTiles = new int[cellCount];

        Array.Fill(wallTiles, -1);
        Array.Fill(floorTiles, -1);
    }

    #region Tile Access

    public int GetWallTile(Vector2Int position)
    {
        if (!TryGetIndex(
                position,
                out int index,
                "Wall tile"))
        {
            return -1;
        }

        return wallTiles[index];
    }

    public void SetWallTile(
        Vector2Int position,
        int tileID)
    {
        if (!TryGetIndex(
                position,
                out int index,
                "Wall tile"))
        {
            return;
        }

        wallTiles[index] = tileID;
    }

    public int GetFloorTile(Vector2Int position)
    {
        if (!TryGetIndex(
                position,
                out int index,
                "Floor tile"))
        {
            return -1;
        }

        return floorTiles[index];
    }

    public void SetFloorTile(
        Vector2Int position,
        int tileID)
    {
        if (!TryGetIndex(
                position,
                out int index,
                "Floor tile"))
        {
            return;
        }

        floorTiles[index] = tileID;
    }

    public void SetCell(
        Vector2Int position,
        MapCell cell)
    {
        if (!TryGetIndex(
                position,
                out int index,
                "Cell"))
        {
            return;
        }

        wallTiles[index] = cell.WallID;
        floorTiles[index] = cell.FloorID;
    }

    #endregion

    #region Bounds / Indexing

    public bool InBounds(Vector2Int position)
    {
        return position.x >= 0 &&
               position.x < ChunkSize &&
               position.y >= 0 &&
               position.y < ChunkSize;
    }

    private int GetIndex(Vector2Int position)
    {
        return position.y * ChunkSize + position.x;
    }

    private bool TryGetIndex(
        Vector2Int position,
        out int index,
        string context)
    {
        if (!InBounds(position))
        {
            Debug.LogError(
                $"{context} location out of bounds: {position}"
            );

            index = -1;
            return false;
        }

        index = GetIndex(position);
        return true;
    }

    #endregion
}