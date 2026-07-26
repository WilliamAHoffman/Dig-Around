using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0)] private int generateRange = 1;
    [SerializeField, Min(0)] private int savedRange = 2;

    private readonly HashSet<Vector2Int> loadedChunks = new();
    private readonly HashSet<Vector2Int> requiredChunks = new();
    private readonly HashSet<Vector2Int> chunksToUnload = new();

    private Vector2Int previousChunkPosition;
    private int previousChunkRange = -1;
    private bool initialized;

    private ChunkManager ChunkManager =>
        GameController.Instance != null
            ? GameController.Instance.ChunkManager
            : null;

    private void Update()
    {
        ChunkManager chunkManager = ChunkManager;

        if (target == null || chunkManager == null)
        {
            return;
        }

        Vector2Int currentChunkPosition =
            ChunkUtilities.WorldToChunkCoord(
                target.position,
                chunkManager.ChunkSize
            );

        bool chunkChanged =
            !initialized ||
            currentChunkPosition != previousChunkPosition;

        bool rangeChanged =
            !initialized ||
            generateRange != previousChunkRange;

        if (!chunkChanged && !rangeChanged)
        {
            return;
        }

        previousChunkPosition = currentChunkPosition;
        previousChunkRange = generateRange;
        initialized = true;

        UpdateLoadedChunks(currentChunkPosition);
    }

    private void UpdateLoadedChunks(Vector2Int center)
    {
        ChunkManager chunkManager = ChunkManager;

        if (chunkManager == null)
        {
            return;
        }

        requiredChunks.Clear();
        chunksToUnload.Clear();

        Vector2Int startLoad = new(
            center.x - generateRange,
            center.y - generateRange
        );

        Vector2Int endLoad = new(
            center.x + generateRange,
            center.y + generateRange
        );

        Vector2Int startSave = new(
            center.x - savedRange,
            center.y - savedRange
        );

        Vector2Int endSave = new(
            center.x + savedRange,
            center.y + savedRange
        );

        for (int x = startLoad.x; x <= endLoad.x; x++)
        {
            for (int y = startLoad.y; y <= endLoad.y; y++)
            {
                requiredChunks.Add(new Vector2Int(x, y));
            }
        }

        // Do not modify a HashSet while iterating over it.
        foreach (Vector2Int chunkPosition in loadedChunks)
        {
            if (chunkPosition.x < startSave.x || chunkPosition.x > endSave.x || chunkPosition.y < startSave.y || chunkPosition.y > endSave.y)
            {
                chunksToUnload.Add(chunkPosition);
            }
        }

        foreach (Vector2Int chunkPosition in chunksToUnload)
        {
            chunkManager.UnloadChunk(chunkPosition);
            loadedChunks.Remove(chunkPosition);
        }

        // Queue only newly required chunks instead of the entire box.
        foreach (Vector2Int chunkPosition in requiredChunks)
        {
            if (loadedChunks.Add(chunkPosition))
            {
                chunkManager.QueueChunkForLoading(
                    chunkPosition,
                    render: true
                );
            }
        }
    }
    
    private void OnDisable()
    {
        ChunkManager chunkManager = ChunkManager;

        if (chunkManager != null)
        {
            foreach (Vector2Int chunkPosition in loadedChunks)
            {
                chunkManager.UnloadChunk(chunkPosition);
            }
        }

        loadedChunks.Clear();
        requiredChunks.Clear();
        chunksToUnload.Clear();

        initialized = false;
        previousChunkRange = -1;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        generateRange = Mathf.Max(0, generateRange);
        savedRange = Mathf.Max(0, savedRange);

        if(savedRange < generateRange)
        {
            savedRange = generateRange;
        }

        if (Application.isPlaying)
        {
            previousChunkRange = -1;
        }
    }
#endif
}
