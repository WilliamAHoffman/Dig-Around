using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Chunk Ranges")]
    [SerializeField, Min(0)]
    private int generateRange = 1;

    [SerializeField, Min(0)]
    private int savedRangeIncrease = 1;

    private readonly HashSet<Vector2Int> trackedChunks = new();
    private readonly HashSet<Vector2Int> requiredChunks = new();

    private readonly List<Vector2Int> chunksToLoad = new();
    private readonly List<Vector2Int> chunksToUnload = new();

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

        if (target == null ||
            chunkManager == null)
        {
            return;
        }

        Vector2Int currentChunkPosition =
            ChunkUtilities.WorldToChunkCoord(
                target.position,
                chunkManager.ChunkSize
            );

        if (!RequiresUpdate(currentChunkPosition))
        {
            return;
        }

        CacheCurrentState(currentChunkPosition);

        UpdateChunks(
            chunkManager,
            currentChunkPosition
        );
    }

    #region Updating

    private bool RequiresUpdate(
        Vector2Int currentChunkPosition)
    {
        if (!initialized)
        {
            return true;
        }

        if (currentChunkPosition != previousChunkPosition)
        {
            return true;
        }

        if (generateRange != previousGenerateRange)
        {
            return true;
        }

        return savedRangeIncrease !=
               previousSavedRangeIncrease;
    }

    private void CacheCurrentState(
        Vector2Int currentChunkPosition)
    {
        previousChunkPosition =
            currentChunkPosition;

        previousGenerateRange =
            generateRange;

        previousSavedRangeIncrease =
            savedRangeIncrease;

        initialized = true;
    }

    private void UpdateChunks(
        ChunkManager chunkManager,
        Vector2Int center)
    {
        ClearWorkingCollections();

        int generateRangeSquared =
            generateRange * generateRange;

        int savedRange =
            generateRange + savedRangeIncrease;

        int savedRangeSquared =
            savedRange * savedRange;

        BuildRequiredChunks(
            center,
            generateRange,
            generateRangeSquared
        );

        FindChunksToUnload(
            center,
            savedRangeSquared
        );

        FindChunksToLoad();

        UnloadChunks(chunkManager);

        SortChunksByDistance(
            chunksToLoad,
            center
        );

        LoadChunks(chunkManager);
    }

    private void ClearWorkingCollections()
    {
        requiredChunks.Clear();
        chunksToLoad.Clear();
        chunksToUnload.Clear();
    }

    #endregion

    #region Required Chunks

    private void BuildRequiredChunks(
        Vector2Int center,
        int radius,
        int radiusSquared)
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

    #endregion

    #region Loading

    private void FindChunksToLoad()
    {
        foreach (Vector2Int chunkPosition in requiredChunks)
        {
            if (trackedChunks.Contains(chunkPosition))
            {
                if (ChunkManager.IsChunkRendered(chunkPosition))
                {
                    continue;
                }
            }

            chunksToLoad.Add(chunkPosition);
        }
    }

    private void LoadChunks(
        ChunkManager chunkManager)
    {
        foreach (Vector2Int chunkPosition in chunksToLoad)
        {
            trackedChunks.Add(chunkPosition);

            chunkManager.QueueChunkForLoading(
                chunkPosition,
                true
            );
        }
    }

    #endregion

    #region Unloading

    private void FindChunksToUnload(
        Vector2Int center,
        int savedRangeSquared)
    {
        foreach (Vector2Int chunkPosition in trackedChunks)
        {
            if (SquaredDistance(
                    chunkPosition,
                    center) <= savedRangeSquared)
            {
                continue;
            }

            chunksToUnload.Add(chunkPosition);
        }
    }

    private void UnloadChunks(
        ChunkManager chunkManager)
    {
        foreach (Vector2Int chunkPosition in chunksToUnload)
        {
            chunkManager.UnloadChunk(chunkPosition);

            trackedChunks.Remove(chunkPosition);
        }
    }

    #endregion

    #region Sorting

    private static void SortChunksByDistance(
        List<Vector2Int> chunks,
        Vector2Int center)
    {
        chunks.Sort(
            (first, second) =>
                SquaredDistance(first, center)
                    .CompareTo(
                        SquaredDistance(second, center)
                    )
        );
    }

    private static int SquaredDistance(
        Vector2Int first,
        Vector2Int second)
    {
        int xDifference =
            first.x - second.x;

        int yDifference =
            first.y - second.y;

        return xDifference * xDifference +
               yDifference * yDifference;
    }

    #endregion

    #region Configuration

    public void SetGenerateRange(int range)
    {
        range = Mathf.Max(0, range);

        if (generateRange == range)
        {
            return;
        }

        generateRange = range;
        initialized = false;
    }

    public void SetSavedRangeIncrease(int increase)
    {
        increase = Mathf.Max(0, increase);

        if (savedRangeIncrease == increase)
        {
            return;
        }

        savedRangeIncrease = increase;
        initialized = false;
    }

    #endregion

    #region Lifecycle

    private void OnDisable()
    {
        UnloadAllTrackedChunks();
        ResetLoaderState();
    }

    private void UnloadAllTrackedChunks()
    {
        ChunkManager chunkManager = ChunkManager;

        if (chunkManager == null)
        {
            return;
        }

        foreach (Vector2Int chunkPosition in trackedChunks)
        {
            chunkManager.UnloadChunk(chunkPosition);
        }
    }

    private void ResetLoaderState()
    {
        trackedChunks.Clear();
        requiredChunks.Clear();

        chunksToLoad.Clear();
        chunksToUnload.Clear();

        initialized = false;

        previousGenerateRange = -1;
        previousSavedRangeIncrease = -1;
    }

    #endregion

#if UNITY_EDITOR

    private void OnValidate()
    {
        generateRange =
            Mathf.Max(0, generateRange);

        savedRangeIncrease =
            Mathf.Max(0, savedRangeIncrease);

        if (Application.isPlaying)
        {
            initialized = false;
        }
    }

#endif
}