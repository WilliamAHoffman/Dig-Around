using System;
using UnityEngine;

[Serializable]
public class ChanceCondition
{
    [SerializeField, Range(0f, 1f)] private float probability = 0.5f;
    [SerializeField] private int salt = 0;

    public bool Evaluate(in GenerationContext context)
    {
        uint hash = Hash(context.Position.x, context.Position.y, context.Seed, salt);
        float value = hash / (float)uint.MaxValue;
        return value < probability;
    }

    private static uint Hash(int x, int y, int seed, int salt)
    {
        unchecked
        {
            uint h = 2166136261u;
            h = (h ^ (uint)x) * 16777619u;
            h = (h ^ (uint)y) * 16777619u;
            h = (h ^ (uint)seed) * 16777619u;
            h = (h ^ (uint)salt) * 16777619u;
            return h;
        }
    }
}
