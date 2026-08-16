using System;
using System.Collections.Generic;
using UnityEngine;

public enum WorldSampleType
{
    Elevation,
    Temperature
}

#region World Sample

[Serializable]
public readonly struct WorldSample
{
    public readonly float Elevation;
    public readonly float Temperature;

    public WorldSample(
        float elevation,
        float temperature)
    {
        Elevation = elevation;
        Temperature = temperature;
    }

    public float GetValue(WorldSampleType type)
    {
        return type switch
        {
            WorldSampleType.Elevation =>
                Elevation,

            WorldSampleType.Temperature =>
                Temperature,

            _ => 0f
        };
    }
}

#endregion

#region Target World Sample

[Serializable]
public class TargetWorldSample
{
    [SerializeField]
    private List<TargetWorldSampleEntry> targets = new();

    public IReadOnlyList<TargetWorldSampleEntry> Targets =>
        targets;

    public bool TryGetEntry(
        WorldSampleType type,
        out TargetWorldSampleEntry entry)
    {
        if (targets != null)
        {
            foreach (TargetWorldSampleEntry target in targets)
            {
                if (target.type != type)
                {
                    continue;
                }

                entry = target;
                return true;
            }
        }

        entry = default;

        return false;
    }

    public TargetWorldSampleEntry GetEntry(
        WorldSampleType type)
    {
        if (TryGetEntry(type, out TargetWorldSampleEntry entry))
        {
            return entry;
        }

        Debug.LogError(
            $"Target world sample does not contain {type}."
        );

        return default;
    }

    public bool HasEntry(WorldSampleType type)
    {
        return TryGetEntry(
            type,
            out _
        );
    }
}

#endregion

#region Sample Entries

[Serializable]
public struct WorldSampleEntry
{
    public WorldSampleType type;

    [Range(-1f, 1f)]
    public float value;
}

[Serializable]
public struct TargetWorldSampleEntry
{
    public WorldSampleType type;

    [Range(-1f, 1f)]
    public float value;

    [Min(0f)]
    public float importance;
}

#endregion