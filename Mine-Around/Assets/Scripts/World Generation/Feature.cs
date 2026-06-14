using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature", menuName = "Scriptable Objects/Feature")]
public class Feature : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Feature;
    public List<GenerationRule> rules;
    public List<float> ruleRanges;
    public NoiseSettings featureNoise;

    public LocationTiles GenerateTiles(LocationTiles currentTiles, Vector2Int location)
    {

        if (featureNoise == null)
        {
            Debug.LogError($"Feature noise has not been initialized for feature: {nameID}", this);
            return currentTiles;
        }

        if (rules == null || ruleRanges == null || rules.Count == 0 || ruleRanges.Count == 0)
        {
            return currentTiles;
        }

        float noiseSample = featureNoise.Sample(location.x, location.y);

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

