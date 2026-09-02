using System;
using UnityEngine;

[Serializable]
public class ExistingFloorCondition
{
    [SerializeField] private BloxelBase requiredFloor;
    [SerializeField] private bool invert;

    public bool Evaluate(in GenerationContext context)
    {
        if (requiredFloor == null)
            return false;

        bool matches = context.Cell.FloorID == requiredFloor.ID;
        return invert ? !matches : matches;
    }
}
