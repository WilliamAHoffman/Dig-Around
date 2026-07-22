using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData", menuName = "Tiles/TileData")]
public class TileData : DatabaseAsset
{
    public Tile tile;
    public int maxHealth;
    public bool invincible;
    public bool transparent;
    public Color mapColor;
}