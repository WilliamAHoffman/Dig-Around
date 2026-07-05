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

    //returns a float from 0 to 1 that represents the similarity of a feature to a world sample
    //takes a worldsample which is a collection of noise values from -1 to 1
    public float Similarity(WorldSample worldSample)
    {
        if (idealSample == null || idealSample == null || idealSample.targets.Count == 0)
            return 0f;

        float weightedDifference = 0f;
        float totalImportance = 0f;

        foreach (TargetWorldSampleEntry target in idealSample.targets)
        {
            float importance = target.importance;

            if (importance <= 0f)
                continue;

            float idealValue = target.value;
            float worldValue = worldSample.GetValue(target.type);

            float difference = Mathf.Abs(idealValue - worldValue);

            weightedDifference += difference * importance;
            totalImportance += importance;
        }

        if (totalImportance <= 0f)
            return 0f;

        float averageDifference = weightedDifference / totalImportance;

        return 1f - averageDifference;
    }
}