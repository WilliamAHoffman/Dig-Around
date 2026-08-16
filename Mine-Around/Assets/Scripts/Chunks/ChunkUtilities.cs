using System;
using UnityEngine;

public static class ChunkUtilities
{
    public static Vector2Int WorldToChunkCoord(
        Vector3 worldPosition,
        int chunkSize)
    {
        ValidateChunkSize(chunkSize);

        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / chunkSize),
            Mathf.FloorToInt(worldPosition.y / chunkSize)
        );
    }

    public static Vector2Int WorldToLocalCoord(
        Vector3 worldPosition,
        int chunkSize)
    {
        ValidateChunkSize(chunkSize);

        Vector2Int blockPosition =
            WorldToBlockCoord(worldPosition);

        Vector2Int chunkPosition =
            WorldToChunkCoord(
                worldPosition,
                chunkSize
            );

        return new Vector2Int(
            blockPosition.x - chunkPosition.x * chunkSize,
            blockPosition.y - chunkPosition.y * chunkSize
        );
    }

    public static Vector2Int WorldToBlockCoord(
        Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x),
            Mathf.FloorToInt(worldPosition.y)
        );
    }

    public static Vector2Int LocalToWorldCoord(
        Vector2Int localPosition,
        Vector2Int chunkPosition,
        int chunkSize)
    {
        ValidateChunkSize(chunkSize);

        return new Vector2Int(
            chunkPosition.x * chunkSize + localPosition.x,
            chunkPosition.y * chunkSize + localPosition.y
        );
    }

    private static void ValidateChunkSize(int chunkSize)
    {
        if (chunkSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(chunkSize),
                chunkSize,
                "Chunk size must be greater than zero."
            );
        }
    }
}