using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(
    fileName = "BloxelBase",
    menuName = "World Data/Bloxels/BloxelBase"
)]
public class BloxelBase : DatabaseAsset
{
    public override string pathName => "bloxel:";

    [SerializeField] private BloxelFloorProperties floorProperties;
    [SerializeField] private BloxelWallProperties wallProperties;

    [Header("Rendering")]
    [SerializeField] private Color mapColor = Color.white;
    [SerializeField] private bool isTransparent;

    public Color MapColor => mapColor;
    public bool IsTransparent => isTransparent;

    public TileBase GetLayerTile(Vector2Int position, TileMapLayer tileMapLayer)
    {
        BloxelLayerProperties properties = SelectLayer(tileMapLayer);

        if(properties == null)
        {
            Debug.LogError("Bloxel does not contain this layer", this);
            return null;
        }

        int randomSeed = GameRandomness.Hash(GetObjectSeed(), position.x, position.y);

        TileBase selected = WeightedRandomSelector.GetWeightedRandom<TileBase>(
            properties.tiles,
            randomSeed
        );

        return selected;
    }

    private BloxelLayerProperties SelectLayer(TileMapLayer tileMapLayer)
    {
        switch (tileMapLayer)
        {
            case TileMapLayer.Floor:
                return floorProperties;
            case TileMapLayer.Wall:
                return wallProperties;
        }

        return null;
    }

    public bool SupportsLayer(TileMapLayer tileMapLayer)
    {
        switch (tileMapLayer)
        {
            case TileMapLayer.Floor:
                return floorProperties;
            case TileMapLayer.Wall:
                return wallProperties;
        }

        return false;
    }

    public virtual void OnPlaced(BloxelContext context)
    {
    }

    public virtual void OnBroken(BloxelContext context)
    {
    }

    public virtual void OnRandomTick(BloxelContext context)
    {
    }
}