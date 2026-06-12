using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Tile;
    public Tile tile;
    public int maxHealth;
    public bool invincible;

    public WorldTile WorldTile()
    {
        return new WorldTile(nameID, maxHealth);
    }
}