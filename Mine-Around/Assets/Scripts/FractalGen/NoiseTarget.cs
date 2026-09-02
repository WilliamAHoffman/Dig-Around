using System;
using UnityEngine;

[Serializable]
public class NoiseTarget
{
    [SerializeField] private NoiseSettings noise;
    [SerializeField, Range(-1f, 1f)] private float noiseTarget = 0f;
    [SerializeField, Min(0f)] private float strength = 1f;

    public float Strength => strength;

    public bool IsValid => noise != null && strength > 0f;

    public float GetStrength(int x, int y)
    {
        if (noise == null || strength <= 0f)
            return 0f;

        float sample = noise.Sample(x, y);
        float difference = Mathf.Abs(sample - noiseTarget);
        float similarity = Mathf.Clamp01(1f - difference);

        return similarity * strength;
    }
}
