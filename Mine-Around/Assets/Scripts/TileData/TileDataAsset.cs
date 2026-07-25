using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[CreateAssetMenu(fileName = "TileDataAsset", menuName = "WorldDataObject/TileDataAsset")]
public class TileDataAsset : DatabaseAsset
{
    public override Type RegistryType => typeof(TileDataAsset);

    [Header("Rendering")]
    public TileBase tile;

    [Header("General Behavior")]
    [SerializeField] private bool isTransparent;
    [SerializeField] private Color mapColor;
    public bool IsTransparent => isTransparent;

    public Color MapColor => mapColor;

    public virtual void OnPlaced(TileContext context)
    {
    }

    public virtual void OnBroken(TileContext context)
    {
    }

    public virtual void OnInteract(TileContext context)
    {
    }

    public virtual void OnRandomTick(TileContext context)
    {
    }

}