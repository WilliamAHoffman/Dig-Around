using System.Collections.Generic;
using UnityEngine;

public enum WorldSampleType
{
    elevation,
    temperature
}

public class NoiseSample
{
    public Dictionary<WorldSampleType, float> dict = new Dictionary<WorldSampleType, float>();
}

[CreateAssetMenu(fileName = "WorldLayers", menuName = "Scriptable Objects/WorldLayers")]
public class NoiseSampler : WorldDataObject
{
    public NoiseSettings elevationNoiseSettings;
    public NoiseSettings temperatureNoiseSettings;

    public NoiseSample Sample(Vector2Int worldPos)
    {
        NoiseSample sample = new NoiseSample();
        
        return sample;
    }
}