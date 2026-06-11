using UnityEngine;

public class WorldTile : ScriptableObject
{
    
}

[CreateAssetMenu(fileName = "WorldWallTile", menuName = "Scriptable Objects/WorldWallTile")]
public class WorldWallTile : WorldTile
{
    public string nameID;
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
    public string nameID;
    public int health;
    public WorldFloorTile(string nameID, int health)
    {
        this.nameID = nameID;
        this.health = health;
    }
}