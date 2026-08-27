using UnityEngine;

[CreateAssetMenu(
    fileName = "WorldGenerationRule",
    menuName = "World Generation/World Spawn Rule"
)]
public class WorldGenerationRule : ScriptableObject
{
    [Header("Target Environment")]
    [SerializeField]
    private TargetWorldSample idealSample;

    [Header("Requirements")]
    [SerializeField, Range(0f, 1f)] private float minPercentSimilarity = 0.5f;

    public float MinSimilarity =>
        minPercentSimilarity;

    public float Similarity(WorldSample worldSample)
    {
        if (idealSample == null ||
            idealSample.Targets == null ||
            idealSample.Targets.Count == 0)
        {
            return 0f;
        }

        float weightedDifference = 0f;
        float totalImportance = 0f;

        foreach (TargetWorldSampleEntry target
                 in idealSample.Targets)
        {
            float importance = target.importance;

            if (importance <= 0f)
            {
                continue;
            }

            float worldValue =
                worldSample.GetValue(target.type);

            float difference =
                Mathf.Abs(
                    target.value -
                    worldValue
                );

            weightedDifference +=
                difference * importance;

            totalImportance += importance;
        }

        if (totalImportance <= 0f)
        {
            return 0f;
        }

        float averageDifference =
            weightedDifference /
            totalImportance;

        return 1f - averageDifference;
    }

    public bool CanGenerate(
        WorldSample worldSample)
    {
        return Similarity(worldSample) >=
               minPercentSimilarity;
    }
}