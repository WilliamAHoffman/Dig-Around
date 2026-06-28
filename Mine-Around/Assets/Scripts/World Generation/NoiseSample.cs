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

    public float Get(WorldSampleType type)
    {
        if (!keys.Contains(type))
        {
            Debug.LogError("World sample does not contain this type");
            return 0;
        }
        return values[keys.IndexOf(type)];
    }
}