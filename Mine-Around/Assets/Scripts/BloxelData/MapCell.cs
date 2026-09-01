using System;

[Serializable]
public struct MapCell
{
    public int FloorID;
    public int WallID;

    public MapCell(int floorID, int wallID)
    {
        FloorID = floorID;
        WallID = wallID;
    }

    public void SetFloor(BloxelBase floor)
    {
        if (floor != null)
            FloorID = floor.ID;
    }

    public void SetWall(BloxelBase wall)
    {
        if (wall != null)
            WallID = wall.ID;
    }

    public void Apply(BloxelBase floor, BloxelBase wall)
    {
        SetFloor(floor);
        SetWall(wall);
    }
}