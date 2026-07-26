using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    floor,
    wall
}

[CreateAssetMenu(
    fileName = "TileDataAsset",
    menuName = "World Data/Tile"
)]
public class TileDataAsset : DatabaseAsset
{
    public override Type RegistryType => typeof(TileDataAsset);

    [Header("Classification")]
    [SerializeField] private TileType tileType;

    [Header("Rendering")]
    [SerializeField] private TileBase tile;
    [SerializeField] private Color mapColor = Color.white;
    [SerializeField] private bool isTransparent;

    [Header("Movement and Visibility")]
    [SerializeField] private bool blocksMovement;
    [SerializeField] private bool blocksVision;

    public TileType TileType => tileType;

    public TileBase Tile => tile;
    public Color MapColor => mapColor;
    public bool IsTransparent => isTransparent;

    public bool BlocksMovement => blocksMovement;
    public bool BlocksVision => blocksVision;

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