using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DecorationRule
{
    [Header("Noise Conditions")]
    [SerializeField] private List<NoiseRangeCondition> noiseRangeConditions = new();
    [SerializeField] private List<NoiseSimilarityCondition> noiseSimilarityConditions = new();

    [Header("Cell Conditions")]
    [SerializeField] private List<ExistingFloorCondition> existingFloorConditions = new();
    [SerializeField] private List<ExistingWallCondition> existingWallConditions = new();

    [Header("Random Conditions")]
    [SerializeField] private List<ChanceCondition> chanceConditions = new();

    [Header("Actions")]
    [SerializeField] private List<SetTileAction> setTileActions = new();

    public bool TryApply(ref GenerationContext context)
    {
        if (noiseRangeConditions != null)
        {
            foreach (NoiseRangeCondition condition in noiseRangeConditions)
            {
                if (condition != null && !condition.Evaluate(context))
                    return false;
            }
        }

        if (noiseSimilarityConditions != null)
        {
            foreach (NoiseSimilarityCondition condition in noiseSimilarityConditions)
            {
                if (condition != null && !condition.Evaluate(context))
                    return false;
            }
        }

        if (existingFloorConditions != null)
        {
            foreach (ExistingFloorCondition condition in existingFloorConditions)
            {
                if (condition != null && !condition.Evaluate(context))
                    return false;
            }
        }

        if (existingWallConditions != null)
        {
            foreach (ExistingWallCondition condition in existingWallConditions)
            {
                if (condition != null && !condition.Evaluate(context))
                    return false;
            }
        }

        if (chanceConditions != null)
        {
            foreach (ChanceCondition condition in chanceConditions)
            {
                if (condition != null && !condition.Evaluate(context))
                    return false;
            }
        }

        if (setTileActions != null)
        {
            foreach (SetTileAction action in setTileActions)
            {
                if (action != null)
                    action.Apply(ref context);
            }
        }

        return true;
    }
}
