using UnityEngine;

public abstract class GenerationFeature : ScriptableObject
{
    public WorldSample idealSample;

    [Range(0f, 1f)]
    public float minSimilarity = 0.5f;

    public abstract GenerationResult Apply(
        Vector2Int location,
        float strength,
        GenerationResult result
    );

    public float Similarity(WorldSample worldSample)
    {
        if (idealSample == null || idealSample.keys == null || idealSample.keys.Count == 0)
            return 0f;

        float totalDifference = 0f;
        int count = 0;

        foreach (WorldSampleType type in idealSample.keys)
        {
            float idealValue = idealSample.Get(type);
            float worldValue = worldSample.Get(type);

            totalDifference += Mathf.Abs(idealValue - worldValue);
            count++;
        }

        if (count == 0)
            return 0f;

        float averageDifference = totalDifference / count;

        return 1f - Mathf.Clamp01(averageDifference);
    }
}