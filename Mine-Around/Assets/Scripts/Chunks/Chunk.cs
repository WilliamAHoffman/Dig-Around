using System;
using UnityEngine;

public enum ChunkState
{
    Ungenerated,
    Queued,
    Cancelled,
    Generated,
    Rendered
}

public class Chunk
{
    public int ChunkSize { get; }
    public ChunkState State { get; internal set; }

    private readonly int[] worldWallTiles;
    private readonly int[] worldFloorTiles;

    public Chunk(
        int size,
        ChunkState state = ChunkState.Ungenerated
    )
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

        worldWallTiles = new int[size * size];
        worldFloorTiles = new int[size * size];
    }

    private int Index(Vector2Int position)
    {
        return position.y * ChunkSize + position.x;
    }

    public bool InBounds(Vector2Int position)
    {
        return position.x >= 0 &&
               position.x < ChunkSize &&
               position.y >= 0 &&
               position.y < ChunkSize;
    }

    public int GetWallTile(Vector2Int position)
    {
        if (!InBounds(position))
        {
            Debug.LogError(
                $"Wall tile location out of bounds: {position}"
            );

            return -1;
        }

        return worldWallTiles[Index(position)];
    }

    public void SetWallTile(Vector2Int position, int tile)
    {
        if (!InBounds(position))
        {
            Debug.LogError(
                $"Wall tile location out of bounds: {position}"
            );

            return;
        }

        worldWallTiles[Index(position)] = tile;
    }

    public int GetFloorTile(Vector2Int position)
    {
        if (!InBounds(position))
        {
            Debug.LogError(
                $"Floor tile location out of bounds: {position}"
            );

            return -1;
        }

        return worldFloorTiles[Index(position)];
    }

    public void SetFloorTile(Vector2Int position, int tile)
    {
        if (!InBounds(position))
        {
            Debug.LogError(
                $"Floor tile location out of bounds: {position}"
            );

            return;
        }

        worldFloorTiles[Index(position)] = tile;
    }

    public void SetChunkCellValues(
        Vector2Int position,
        MapCell cell
    )
    {
        if (!InBounds(position))
        {
            Debug.LogError(
                $"Tile location out of bounds: {position}"
            );

            return;
        }

        int index = Index(position);

        worldWallTiles[index] = cell.WallID;
        worldFloorTiles[index] = cell.FloorID;
    }
}