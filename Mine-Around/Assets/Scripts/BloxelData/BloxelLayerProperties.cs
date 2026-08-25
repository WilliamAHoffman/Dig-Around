using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public abstract class BloxelLayerProperties : ScriptableObject
{
    public List<WeightedItem<TileBase>> tiles;
}
