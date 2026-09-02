using System;
using UnityEngine;

[Serializable]
public class ExistingWallCondition
{
    [SerializeField] private BloxelBase requiredWall;
    [SerializeField] private bool invert;

    public bool Evaluate(in GenerationContext context)
    {
        if (requiredWall == null)
            return false;

        bool matches = context.Cell.WallID == requiredWall.ID;
        return invert ? !matches : matches;
    }
}
