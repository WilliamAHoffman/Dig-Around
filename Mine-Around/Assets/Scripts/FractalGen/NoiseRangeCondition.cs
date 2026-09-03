using System;
using UnityEngine;

[Serializable]
public class NoiseRangeCondition
{
    [SerializeField] private NoiseSettings noise;
    [SerializeField, Range(-1f, 1f)] private float minValue = -1f;
    [SerializeField, Range(-1f, 1f)] private float maxValue = 1f;
    [SerializeField] private bool invert;

    public bool Evaluate(in GenerationContext context)
    {
        if (noise == null)
            return false;

        float min = Mathf.Min(minValue, maxValue);
        float max = Mathf.Max(minValue, maxValue);
        float sample = noise.Sample(context.Position);

        bool inside = sample >= min && sample <= max;
        return invert ? !inside : inside;
    }
}
