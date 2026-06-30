using UnityEngine;

public abstract class GenerationFeature : ScriptableObject
{
    public TargetWorldSample idealSample;

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

        float weightedDifference = 0f;
        float totalImportance = 0f;

        foreach (WorldSampleType type in idealSample.keys)
        {
            float importance = idealSample.GetImportance(type);

            if (importance <= 0f)
                continue;

            float idealValue = idealSample.GetValue(type);
            float worldValue = worldSample.GetValue(type);

            float difference = Mathf.Abs(idealValue - worldValue);

            weightedDifference += difference * importance;
            totalImportance += importance;
        }

        if (totalImportance <= 0f)
            return 0f;

        float averageDifference = weightedDifference / totalImportance;

        return 1f - Mathf.Clamp01(averageDifference);
    }
}