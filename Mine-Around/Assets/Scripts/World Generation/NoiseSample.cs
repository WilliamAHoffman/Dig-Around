using System;
using System.Collections.Generic;
using UnityEngine;

//all go with NoiseSampler
public enum WorldSampleType
{
    elevation,
    temperature
}
[Serializable]
public class WorldSample
{
    [SerializeField] public List<WorldSampleType> keys = new List<WorldSampleType>();
    [SerializeField] public List<float> values = new List<float>();

    public void Add(WorldSampleType type, float value)
    {
        keys.Add(type);
        values.Add(value);
    }

    public float GetValue(WorldSampleType type)
    {
        if (!keys.Contains(type))
        {
            Debug.LogError("World sample does not contain this type");
            return 0;
        }
        return values[keys.IndexOf(type)];
    }
}

[Serializable]
public class TargetWorldSample
{
    [SerializeField] public List<TargetWorldSampleEntry> targets = new List<TargetWorldSampleEntry>();

    public TargetWorldSampleEntry GetEntry(WorldSampleType type)
    {
        foreach(TargetWorldSampleEntry tws in targets)
        {
            if(tws.type == type)
            {
                return tws;
            }
        }

        Debug.LogError("World sample does not contain this type! (returning: index 0)");
        return targets[0];
    }

    public bool HasEntry(WorldSampleType type)
    {
        foreach(TargetWorldSampleEntry tws in targets)
        {
            if(tws.type == type)
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