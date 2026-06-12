using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature", menuName = "Scriptable Objects/Feature")]
public class Feature : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Feature;
    public List<GenerationRule> rules;
    public FastNoiseLite featureNoise;
    public string GenerateTile(string replacing, Vector2Int location, int seed)
    {
        float noiseSample = featureNoise.GetNoise(location.x, location.y);
        foreach(GenerationRule rule in rules)
        {
            if(rule.Matches(noiseSample) && rule.Replaces(replacing))
            {
                return rule.tileID;
            }
        }
        return replacing;
    }
}

