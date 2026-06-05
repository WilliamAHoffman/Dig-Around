using System.Collections.Generic;
using UnityEngine;
/*
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

    public static Vector2Int WorldToBlockCoord(Vector3 worldPosition)
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

    public static bool InBounds(Vector2Int localPos, int chunkSize)
    {
        return localPos.x >= 0 && localPos.x < chunkSize &&
               localPos.y >= 0 && localPos.y < chunkSize;
    }

    public static bool HasEmptyWorldPos(Vector3 start, Vector3 step, float length)
    {
        int i = 0;
        while (i < length)
        {
            IndexWallTile tile = ChunkManager.Instance.GetIndexWallTileAtLocation(start + step * i);
            if (tile.tileID == 0)
            {
                return true;
            }
            i++;
        }
        return false;
    }

    public static Vector3 FindEmptyWorldPos(Vector3 start, Vector3 step, float length)
    {
        int i = 0;
        List<Vector3> locations = new List<Vector3>();
        while (i < length)
        {
            IndexWallTile tile = ChunkManager.Instance.GetIndexWallTileAtLocation(start + step * i);
            if (tile.tileID == 0)
            {
                locations.Add(start + step * i);
            }
            i++;
        }

        if (locations.Count == 0)
        {
            Debug.LogError("FindEmptyWorldPos: no empty positions found");
            return start;
        }

        return locations[Random.Range(0, locations.Count)];
    }

    public static bool IsEmptyLocation(Vector3 location)
    {
        if (!ChunkManager.Instance.InWorldBounds(location)) return true;
        IndexWallTile tile = ChunkManager.Instance.GetIndexWallTileAtLocation(location);
        if (tile.tileID == 0)
        {
            return true;
        }
        return false;
    }
}

*/