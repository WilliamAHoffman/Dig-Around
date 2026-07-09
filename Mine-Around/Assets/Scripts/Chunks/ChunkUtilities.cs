using UnityEngine;

public static class ChunkUtilities
{
    public static Vector2Int WorldToChunkCoord(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / ChunkManager.Instance.ChunkSize),
            Mathf.FloorToInt(worldPosition.y / ChunkManager.Instance.ChunkSize)
        );
    }

    public static Vector2Int WorldToLocalCoord(Vector3 worldPosition)
    {
        Vector2Int chunkCoord = WorldToChunkCoord(worldPosition);
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x) - chunkCoord.x * ChunkManager.Instance.ChunkSize,
            Mathf.FloorToInt(worldPosition.y) - chunkCoord.y * ChunkManager.Instance.ChunkSize
        );
    }

    public static Vector2Int WorldToBlockCoord(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x),
            Mathf.FloorToInt(worldPosition.y)
        );
    }

    public static Vector2Int LocalToWorldCoord(Vector2Int localPos, Vector2Int chunkCoord)
    {
        return new Vector2Int(
            chunkCoord.x * ChunkManager.Instance.ChunkSize + localPos.x,
            chunkCoord.y * ChunkManager.Instance.ChunkSize + localPos.y
        );
    }
}

