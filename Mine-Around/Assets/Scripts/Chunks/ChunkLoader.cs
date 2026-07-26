using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Chunk Ranges")]
    [SerializeField, Min(0)]
    private int generateRange = 1;

    [SerializeField, Min(0)]
    private int savedRangeIncrease = 1;

    private readonly HashSet<Vector2Int> loadedChunks = new();
    private readonly HashSet<Vector2Int> requiredChunks = new();
    private readonly List<Vector2Int> chunksToUnload = new();
    private readonly List<Vector2Int> chunksToLoad = new();

    private Vector2Int previousChunkPosition;
    private int previousGenerateRange = -1;
    private int previousSavedRangeIncrease = -1;
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
            generateRange != previousGenerateRange ||
            savedRangeIncrease != previousSavedRangeIncrease;

        if (!chunkChanged && !rangeChanged)
        {
            return;
        }

        previousChunkPosition = currentChunkPosition;
        previousGenerateRange = generateRange;
        previousSavedRangeIncrease = savedRangeIncrease;
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
        chunksToLoad.Clear();

        int generateRangeSquared =
            generateRange * generateRange;

        int savedRange =
            generateRange + savedRangeIncrease;

        int savedRangeSquared =
            savedRange * savedRange;

        BuildRequiredChunkCircle(
            center,
            generateRange,
            generateRangeSquared
        );

        FindChunksOutsideSavedRange(
            center,
            savedRangeSquared
        );

        UnloadDistantChunks(chunkManager);

        FindNewChunksToLoad();

        chunksToLoad.Sort(
            (first, second) =>
            {
                int firstDistance =
                    SquaredDistance(first, center);

                int secondDistance =
                    SquaredDistance(second, center);

                return firstDistance.CompareTo(
                    secondDistance
                );
            }
        );

        foreach (Vector2Int chunkPosition in chunksToLoad)
        {
            loadedChunks.Add(chunkPosition);

            chunkManager.QueueChunkForLoading(
                chunkPosition
            );
        }
    }

    private void BuildRequiredChunkCircle(
        Vector2Int center,
        int radius,
        int radiusSquared
    )
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int distanceSquared =
                    x * x + y * y;

                if (distanceSquared > radiusSquared)
                {
                    continue;
                }

                requiredChunks.Add(
                    center + new Vector2Int(x, y)
                );
            }
        }
    }

    private void FindChunksOutsideSavedRange(
        Vector2Int center,
        int savedRangeSquared
    )
    {
        foreach (Vector2Int chunkPosition in loadedChunks)
        {
            int distanceSquared =
                SquaredDistance(
                    chunkPosition,
                    center
                );

            if (distanceSquared > savedRangeSquared)
            {
                chunksToUnload.Add(chunkPosition);
            }
        }
    }

    private void UnloadDistantChunks(
        ChunkManager chunkManager
    )
    {
        foreach (Vector2Int chunkPosition in chunksToUnload)
        {
            chunkManager.UnloadChunk(chunkPosition);
            loadedChunks.Remove(chunkPosition);
        }
    }

    private void FindNewChunksToLoad()
    {
        foreach (Vector2Int chunkPosition in requiredChunks)
        {
            if (!loadedChunks.Contains(chunkPosition))
            {
                chunksToLoad.Add(chunkPosition);
            }
        }
    }

    private static int SquaredDistance(
        Vector2Int first,
        Vector2Int second
    )
    {
        int xDifference =
            first.x - second.x;

        int yDifference =
            first.y - second.y;

        return xDifference * xDifference +
               yDifference * yDifference;
    }

    public void SetGenerateRange(int range)
    {
        generateRange = Mathf.Max(0, range);
        initialized = false;
    }

    public void SetSavedRangeIncrease(int increase)
    {
        savedRangeIncrease = Mathf.Max(0, increase);
        initialized = false;
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
        chunksToLoad.Clear();

        initialized = false;
        previousGenerateRange = -1;
        previousSavedRangeIncrease = -1;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        generateRange = Mathf.Max(
            0,
            generateRange
        );

        savedRangeIncrease = Mathf.Max(
            0,
            savedRangeIncrease
        );

        if (Application.isPlaying)
        {
            initialized = false;
        }
    }
#endif
}