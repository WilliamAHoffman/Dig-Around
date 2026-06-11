using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature", menuName = "Scriptable Objects/Feature")]
public class Feature : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Feature;
    public List<GenerationRule> rules;
    public NoiseSettings featureNoise;
    public string GenerateTile(string replacing, Vector2Int location, int seed)
    {
        float noiseSample = Noise.SampleNoise(location.x,location.y,featureNoise,seed);
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

