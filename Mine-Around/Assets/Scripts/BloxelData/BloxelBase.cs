using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(
    fileName = "TileDataAsset",
    menuName = "World Data/Bloxel"
)]
public class BloxelBase : DatabaseAsset
{
    public override string pathName => "bloxel:";

    [Header("Rendering")]
    [SerializeField] private List<WeightedItem<Tile>> floorTiles;
    [SerializeField] private List<WeightedItem<Tile>> wallTiles;
    [SerializeField] private Color mapColor = Color.white;
    [SerializeField] private bool isTransparent;

    public Color MapColor => mapColor;
    public bool IsTransparent => isTransparent;

    public TileBase GetWallTile(Vector2Int position)
    {
        int randomSeed = GameRandomness.Hash(GetObjectSeed(), position.x, position.y);

        TileBase selected = WeightedRandomSelector.GetWeightedRandom<Tile>(
            wallTiles,
            randomSeed
        );

        return selected;
    }
    public TileBase GetFloorTile(Vector2Int position)
    {
        int randomSeed = GameRandomness.Hash(GetObjectSeed(), position.x, position.y);

        TileBase selected = WeightedRandomSelector.GetWeightedRandom<Tile>(
            floorTiles,
            randomSeed
        );

        return selected;
    }
    public virtual void OnPlaced(BloxelContext context)
    {
    }

    public virtual void OnBroken(BloxelContext context)
    {
    }

    public virtual void OnInteract(BloxelContext context)
    {
    }

    public virtual void OnRandomTick(BloxelContext context)
    {
    }
}