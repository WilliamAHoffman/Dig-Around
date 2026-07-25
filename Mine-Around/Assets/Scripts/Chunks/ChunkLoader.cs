using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0)] private int chunkRange = 1;

    private readonly HashSet<Vector2Int> loadedChunks = new();

    private Vector2Int previousChunkPosition;
    private bool initialized;

    private ChunkManager ChunkManager => GameController.Instance.ChunkManager;

    private void Update()
    {
        if (target == null || ChunkManager == null)
        {
            return;
        }

        Vector2Int chunkPosition = ChunkUtilities.WorldToChunkCoord(
            target.position,
            ChunkManager.ChunkSize
        );

        // Only update when the target enters a different chunk.
        if (initialized && chunkPosition == previousChunkPosition)
        {
            return;
        }

        previousChunkPosition = chunkPosition;
        initialized = true;

        UpdateLoadedChunks(chunkPosition);
    }

    private void UpdateLoadedChunks(Vector2Int center)
    {
        Vector2Int start = new(
            center.x - chunkRange,
            center.y - chunkRange
        );

        Vector2Int end = new(
            center.x + chunkRange,
            center.y + chunkRange
        );

        HashSet<Vector2Int> requiredChunks = new();

        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                requiredChunks.Add(new Vector2Int(x, y));
            }
        }

        // Unload chunks that are no longer in range.
        foreach (Vector2Int chunk in loadedChunks)
        {
            if (!requiredChunks.Contains(chunk))
            {
                ChunkManager.UnloadChunk(chunk);
            }
        }

        ChunkManager.AsyncCreateBox(start, end, true);

        loadedChunks.Clear();
        loadedChunks.UnionWith(requiredChunks);
    }
}