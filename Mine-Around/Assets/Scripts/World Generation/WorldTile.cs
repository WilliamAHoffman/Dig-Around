[System.Serializable]
public class WorldWallTile
{
    public string nameID;
    public int health;

    public WorldWallTile(string nameID, int health)
    {
        this.nameID = nameID;
        this.health = health;
    }
}

[System.Serializable]
public class WorldFloorTile
{
    public string nameID;
    public int health;
    public WorldFloorTile(string nameID, int health)
    {
        this.nameID = nameID;
        this.health = health;
    }
}