using UnityEngine;

public class WorldTile : ScriptableObject
{
    public string nameID;

    public int health;

    public WorldTile(string nameID, int health)
    {
        this.nameID = nameID;
        this.health = health;
    }
}