using System;
using System.Collections.Generic;
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

    private readonly int[] wallBloxelIDs;
    private readonly int[] floorBloxelIDs;

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

        wallBloxelIDs = new int[cellCount];
        floorBloxelIDs = new int[cellCount];

        Array.Fill(wallBloxelIDs, -1);
        Array.Fill(floorBloxelIDs, -1);
    }

    #region Tile Access

    private int[] GetLayerInfo(BloxelLayer bloxelLayer)
    {
        switch (bloxelLayer)
        {
            case BloxelLayer.Floor:
                return floorBloxelIDs;
            case BloxelLayer.Wall:
                return wallBloxelIDs;
        }

        Debug.LogError("Unsupported layer type: " + bloxelLayer);
        return null;
    }

    public int GetBloxelID(Vector2Int position, BloxelLayer bloxelLayer)
    {
        if (!TryGetIndex(
                position,
                out int index,
                "Wall tile"))
        {
            return -1;
        }

        return GetLayerInfo(bloxelLayer)[index];
    }

    public void SetBloxelID(
        Vector2Int position,
        int tileID,
        BloxelLayer bloxelLayer)
    {
        if (!TryGetIndex(
                position,
                out int index,
                "Wall tile"))
        {
            return;
        }


        GetLayerInfo(bloxelLayer)[index] = tileID;
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

        wallBloxelIDs[index] = cell.WallID;
        floorBloxelIDs[index] = cell.FloorID;
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