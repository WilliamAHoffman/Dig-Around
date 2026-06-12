using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Feature", menuName = "Scriptable Objects/Feature")]
public class Feature : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Feature;
    public List<TileData> tiles;
    public List<float> tileRanges;
    private FastNoiseLite featureNoise;
    public NoiseSettings noiseSettings;

    public void SetNoise(int seed)
    {
        featureNoise = noiseSettings.GetNoise(seed);
    }
    public string GenerateTile(string replacing, Vector2Int location)
    {
        float noiseSample = featureNoise.GetNoise(location.x, location.y);
        for(int i = 0; i < tileRanges.Count; i++)
        {
            if(noiseSample - tileRanges[i] <= 0)
            {
                return tiles[i].nameID;
            }
        }
        return replacing;
    }
}

