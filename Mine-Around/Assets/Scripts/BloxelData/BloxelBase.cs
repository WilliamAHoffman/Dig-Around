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
    [SerializeField] private bool isInvisible;

    public Color MapColor => mapColor;
    public bool IsTransparent => isTransparent;
    public bool IsInvisible => isInvisible;

    public TileBase GetLayerTile(Vector2Int position, BloxelLayer bloxelLayer)
    {
        BloxelLayerProperties properties = SelectLayer(bloxelLayer);

        if(properties == null)
        {
            return null;
        }

        int randomSeed = GameRandomness.Hash(GetObjectSeed(), position.x, position.y);

        TileBase selected = WeightedRandomSelector.GetWeightedRandom<TileBase>(
            properties.tiles,
            randomSeed
        );

        return selected;
    }

    private BloxelLayerProperties SelectLayer(BloxelLayer tileMapLayer)
    {
        switch (tileMapLayer)
        {
            case BloxelLayer.Floor:
                return floorProperties;
            case BloxelLayer.Wall:
                return wallProperties;
        }

        return null;
    }

    public bool SupportsLayer(BloxelLayer bloxelLayer)
    {
        switch (bloxelLayer)
        {
            case BloxelLayer.Floor:
                return floorProperties;
            case BloxelLayer.Wall:
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