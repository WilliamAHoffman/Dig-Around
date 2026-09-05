using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Existing Floor Condition",
    menuName = "FractalGen/Condition/Existing Floor Condition"
)]
public class ExistingFloorCondition : GenerationCondition
{
    [SerializeField] private BloxelBase requiredFloor;
    [SerializeField] private bool invert;

    public override bool Evaluate(in GenerationContext context)
    {
        if (requiredFloor == null)
            return false;

        bool matches = context.Cell.FloorID == requiredFloor.ID;
        return invert ? !matches : matches;
    }
}
