using System;

[Serializable]
public struct MapCell
{
    public int FloorID;
    public int WallID;
    
    public MapCell(int FloorID, int WallID)
    {
        this.FloorID = FloorID;
        this.WallID = WallID;
    }
}
