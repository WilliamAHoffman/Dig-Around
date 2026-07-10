using UnityEngine;

public static class ChunkUtilities
{
    public static Vector2Int WorldToChunkCoord(Vector3 worldPosition, int chunkSize)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / chunkSize),
            Mathf.FloorToInt(worldPosition.y / chunkSize)
        );
    }

    public static Vector2Int WorldToLocalCoord(Vector3 worldPosition, int chunkSize)
    {
        Vector2Int chunkCoord = WorldToChunkCoord(worldPosition, chunkSize);
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x) - chunkCoord.x * chunkSize,
            Mathf.FloorToInt(worldPosition.y) - chunkCoord.y * chunkSize
        );
    }

    public static Vector2Int WorldToBlockCoord(Vector3 worldPosition, int chunkSize)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x),
            Mathf.FloorToInt(worldPosition.y)
        );
    }

    public static Vector2Int LocalToWorldCoord(Vector2Int localPos, Vector2Int chunkCoord, int chunkSize)
    {
        return new Vector2Int(
            chunkCoord.x * chunkSize + localPos.x,
            chunkCoord.y * chunkSize + localPos.y
        );
    }
}

