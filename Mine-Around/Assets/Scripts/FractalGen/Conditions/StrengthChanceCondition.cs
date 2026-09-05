using System;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Strength Chance Condition",
    menuName = "FractalGen/Condition/Strength Chance Condition"
)]
public class StrengthChanceCondition : GenerationCondition
{
    [SerializeField, Range(0f, 1f)] private float maxProbability = 0.5f;
    [SerializeField] private int salt = 11223;
    [SerializeField] private NoiseTarget noise;
    [SerializeField, Range(0f, 1f)] private float minStrength;

    public override bool Evaluate(in GenerationContext context)
    {
        uint hash = (uint)math.abs(GameRandomness.Hash(context.Position.x, context.Position.y, context.Seed, salt));
        float value = hash / (float)uint.MaxValue;
        if(value * maxProbability < minStrength) return false;

        bool matches = value * maxProbability > noise.GetStrength(context.Position);
        return matches;
    }
}
