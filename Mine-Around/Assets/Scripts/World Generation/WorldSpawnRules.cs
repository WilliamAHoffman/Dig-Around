using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenerationRule", menuName = "World Generation/WorldSpawnRule")]
public class WorldGenerationRule : ScriptableObject
{
    public TargetWorldSample idealSample;

    [Range(0f, 1f)]
    public float minSimilarity = 0.5f;
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

    public bool CanGenerate(WorldSample worldSample)
    {
        return Similarity(worldSample) >= minSimilarity;
    }
}
