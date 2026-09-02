using System;
using UnityEngine;

[Serializable]
public class SetTileAction
{
    [SerializeField] private BloxelBase floor;
    [SerializeField] private BloxelBase wall;

    public void Apply(ref GenerationContext context)
    {
        context.Cell.Apply(floor, wall);
    }
}
