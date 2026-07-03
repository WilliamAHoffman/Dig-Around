using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData", menuName = "WorldDataObject/TileData")]
public class TileData : WorldDataObject
{
    public Tile tile;
    public int maxHealth;
    public bool invincible;
    public bool transparent;

    public WorldTile WorldTile()
    {
        return new WorldTile(nameID, maxHealth);
    }

    public LocationTiles LocationTiles()
    {
        return new LocationTiles(nameID, nameID);
    }
}