using System;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] public List<WorldSampleType> keys = new List<WorldSampleType>();
    [SerializeField] public List<float> values = new List<float>();
    [SerializeField] public List<float> importance = new List<float>();

    public void Add(WorldSampleType type, float value, float influence)
    {
        keys.Add(type);
        values.Add(value);
        importance.Add(influence);
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

    public float GetImportance(WorldSampleType type)
    {
        if (!keys.Contains(type))
        {
            Debug.LogError("World sample does not contain this type");
            return 0;
        }
        return importance[keys.IndexOf(type)];
    }
}