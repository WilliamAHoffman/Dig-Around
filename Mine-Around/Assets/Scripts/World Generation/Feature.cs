using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature", menuName = "Scriptable Objects/Feature")]
public class Feature : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Feature;
    public List<GenerationRule> rules;
    public List<float> ruleRanges;
    private FastNoiseLite featureNoise;
    public NoiseSettings noiseSettings;

    public void SetNoise(int seed)
    {
        featureNoise = noiseSettings.GetNoise(seed);
    }
    public LocationTiles GenerateTiles(LocationTiles currentTiles, Vector2Int location)
    {

        if (featureNoise == null)
        {
            Debug.LogError($"Feature noise has not been initialized for feature: {nameID}", this);
            return currentTiles;
        }

        float noiseSample = featureNoise.GetNoise(location.x, location.y);

        int count = Mathf.Min(rules.Count, ruleRanges.Count);

        for (int i = 0; i < count; i++)
        {
            if (rules[i] == null)
                continue;

            if (noiseSample <= ruleRanges[i])
            {
                return rules[i].GetNewTiles(currentTiles);
            }
        }

        return currentTiles;
    }
}

