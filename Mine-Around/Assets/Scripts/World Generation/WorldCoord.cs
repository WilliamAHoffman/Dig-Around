using UnityEngine;

public class WorldCoord
{
    public Vector3 globalPosition;
    public Vector2Int chunkPosition;
    public Vector2Int localPosition;

    public WorldCoord(Vector3 position, int chunkSize)
    {
        globalPosition = position;

        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);

        int chunkX = Mathf.FloorToInt((float)x / chunkSize);
        int chunkY = Mathf.FloorToInt((float)y / chunkSize);

        chunkPosition = new Vector2Int(chunkX, chunkY);

        int localX = x - chunkX * chunkSize;
        int localY = y - chunkY * chunkSize;

        localPosition = new Vector2Int(localX, localY);
    }
}