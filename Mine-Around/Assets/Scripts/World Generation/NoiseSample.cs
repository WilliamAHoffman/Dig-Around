using System;
using System.Collections.Generic;
using UnityEngine;

//all go with NoiseSampler
public enum WorldSampleType
{
    Elevation,
    Temperature
}
[Serializable]
public readonly struct WorldSample
{
    public readonly float Elevation;
    public readonly float Temperature;

    public WorldSample(float elevation, float temperature)
    {
        Elevation = elevation;
        Temperature = temperature;
    }

    public float GetValue(WorldSampleType type)
    {
        return type switch
        {
            WorldSampleType.Elevation => Elevation,
            WorldSampleType.Temperature => Temperature,
            _ => 0f
        };
    }
}

[Serializable]
public class TargetWorldSample
{
    [SerializeField] public List<TargetWorldSampleEntry> targets = new List<TargetWorldSampleEntry>();

    public TargetWorldSampleEntry GetEntry(WorldSampleType type)
    {
        foreach (TargetWorldSampleEntry tws in targets)
        {
            if (tws.type == type)
            {
                return tws;
            }
        }

        Debug.LogError("World sample does not contain this type! (returning: index 0)");
        return targets[0];
    }

    public bool HasEntry(WorldSampleType type)
    {
        foreach (TargetWorldSampleEntry tws in targets)
        {
            if (tws.type == type)
            {
                return true;
            }
        }
        Debug.LogError("World sample does not contain this type!");
        return false;
    }
}

[Serializable]
public struct WorldSampleEntry
{
    public WorldSampleType type;
    [Range(-1f, 1f)] public float value;
}

[Serializable]
public struct TargetWorldSampleEntry
{
    public WorldSampleType type;
    [Range(-1f, 1f)] public float value;
    [Min(0f)] public float importance;
}