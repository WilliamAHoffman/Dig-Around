using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Existing Wall Condition",
    menuName = "FractalGen/Condition/Existing Wall Condition"
)]public class ExistingWallCondition : GenerationCondition
{
    [SerializeField] private BloxelBase requiredWall;
    [SerializeField] private bool invert;

    public override bool Evaluate(in GenerationContext context)
    {
        if (requiredWall == null)
            return false;

        bool matches = context.Cell.WallID == requiredWall.ID;
        return invert ? !matches : matches;
    }
}
