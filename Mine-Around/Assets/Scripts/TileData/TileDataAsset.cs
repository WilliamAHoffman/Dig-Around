using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(
    fileName = "TileDataAsset",
    menuName = "World Data/Tile"
)]
public class TileDataAsset : DatabaseAsset
{
    public override Type RegistryType => typeof(TileDataAsset);

    [Header("Rendering")]
    [SerializeField] private List<WeightedItem<Tile>> tiles;
    [SerializeField] private Color mapColor = Color.white;
    [SerializeField] private bool isTransparent;

    [Header("Movement and Visibility")]
    [SerializeField] private bool blocksMovement;
    [SerializeField] private bool blocksVision;

    public Color MapColor => mapColor;
    public bool IsTransparent => isTransparent;

    public bool BlocksMovement => blocksMovement;
    public bool BlocksVision => blocksVision;

    public TileBase GetTile(Vector2Int position)
    {
        int randomSeed = GameRandomness.Hash(GetObjectSeed(), position.x, position.y);

        TileBase selected = WeightedRandomSelector.GetWeightedRandom<Tile>(
            tiles,
            randomSeed
        );

        return selected;
    }
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