using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DecorationRule
{
    [Header("Conditions")]
    [SerializeField] private List<GenerationCondition> conditions = new();

    [Header("Actions")]
    [SerializeField] private List<SetTileAction> setTileActions = new();

    public bool TryApply(ref GenerationContext context)
    {
        if (conditions != null)
        {
            foreach (GenerationCondition condition in conditions)
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
