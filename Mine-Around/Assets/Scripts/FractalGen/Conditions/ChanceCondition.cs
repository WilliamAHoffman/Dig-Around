using System;
using UnityEngine;
using Unity.Mathematics;

[CreateAssetMenu(
    fileName = "Chance Condition",
    menuName = "FractalGen/Condition/Chance Condition"
)]
public class ChanceCondition : GenerationCondition
{
    [SerializeField, Range(0f, 1f)] private float probability = 0.5f;
    [SerializeField] private int salt = 0;

    public override bool Evaluate(in GenerationContext context)
    {
        uint hash = (uint)math.abs(GameRandomness.Hash(context.Position.x, context.Position.y, context.Seed, salt));
        float value = hash / (float)uint.MaxValue;
        return value < probability;
    }
}
