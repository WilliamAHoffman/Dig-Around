[System.Serializable]
public struct WorldWallTile
{
    public string nameID;
    public int health;
    public string floorBelow;

    public WorldWallTile(string nameID, int health, string floorBelow)
    {
        this.nameID = nameID;
        this.health = health;
        this.floorBelow = floorBelow;
    }
}

[System.Serializable]
public struct WorldFloorTile
{
    public string nameID;
    public int health;
    public WorldFloorTile(string nameID, int health)
    {
        this.nameID = nameID;
        this.health = health;
    }
}