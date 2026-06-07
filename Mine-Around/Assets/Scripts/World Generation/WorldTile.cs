using UnityEngine;

public class WorldTile : ScriptableObject
{
    public string nameID;
}

[CreateAssetMenu(fileName = "WorldWallTile", menuName = "Scriptable Objects/WorldWallTile")]
public class WorldWallTile : WorldTile
{
    public int health;

    public WorldWallTile(string nameID, int health)
    {
        this.nameID = nameID;
        this.health = health;
    }
}

[CreateAssetMenu(fileName = "WorldFloorTile", menuName = "Scriptable Objects/WorldFloorTile")]
public class WorldFloorTile : WorldTile
{
    public int health;
    public WorldFloorTile(string nameID, int health)
    {
        this.nameID = nameID;
        this.health = health;
    }
}