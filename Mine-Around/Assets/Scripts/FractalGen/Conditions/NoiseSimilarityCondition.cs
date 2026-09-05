using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Noise Similarity Condition",
    menuName = "FractalGen/Condition/Noise Similarity Condition"
)]
public class NoiseSimilarityCondition : GenerationCondition
{
    [SerializeField] private List<NoiseTarget> targets = new();
    [SerializeField, Range(0f, 1f)] private float requiredSimilarity = 0.5f;

    public override bool Evaluate(in GenerationContext context)
    {
        if (targets == null || targets.Count == 0)
            return false;

        float totalWeight = 0f;
        float totalScore = 0f;

        foreach (NoiseTarget target in targets)
        {
            if (target == null || !target.IsValid)
                continue;

            totalWeight += target.Strength;
            totalScore += target.GetStrength(context.Position);
        }

        if (totalWeight <= Mathf.Epsilon)
            return false;

        return totalScore / totalWeight >= requiredSimilarity;
    }
}
