using UnityEngine;
[CreateAssetMenu(
    fileName = "WallProperties",
    menuName = "World Data/Bloxels/Wall Properties"
)]
public class BloxelWallProperties : BloxelLayerProperties
{
    [SerializeField] private bool isTransparent;
    [SerializeField] private bool isInvisible;

    public bool IsTransparent => isTransparent;
    public bool IsInvisible => isInvisible;
}
