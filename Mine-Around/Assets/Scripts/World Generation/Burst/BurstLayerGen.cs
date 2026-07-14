using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;


[BurstCompile]
public static class BurstLayerGenerator
{
    public static float FeatureSimilarity(
        GenerationFeatureData layer,
        WorldSampleData sample)
    {

        float weightedDifference = 0f;
        float totalImportance = 0f;

        foreach (IdealSample target in layer.Samples)
        {
            float importance = target.Importance;

            if (importance <= 0f)
                continue;

            float idealValue = target.Value;
            float worldValue = sample.GetValue(target.Type);

            float difference = Mathf.Abs(idealValue - worldValue);

            weightedDifference += difference * importance;
            totalImportance += importance;
        }

        if (totalImportance <= 0f)
            return 0f;

        float averageDifference = weightedDifference / totalImportance;

        return 1f - averageDifference;
    }

    //public static LocationTiles PickTile(NativeArray<GenerationFeatureData> featureDatas, )
    //{
    //    
    //}
}