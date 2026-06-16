using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "Scriptable Objects/GenerationRule")]
public class GenerationRule : WorldDataObject
{
    public TileData wall;
    public TileData floor;
    public List<TileData> placeOnWall;
    public List<TileData> placeOnFloor;

    private bool CheckWall(string wall)
    {
        if(placeOnWall == null || placeOnWall.Count == 0) return true;
        foreach(TileData tile in placeOnWall)
        {
            if(tile.nameID == wall)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckFloor(string floor)
    {
        if(placeOnFloor == null || placeOnFloor.Count == 0) return true;
        foreach(TileData tile in placeOnFloor)
        {
            if(tile.nameID == floor)
            {
                return true;
            }
        }

        return false;
    }

    public LocationTiles GetNewTiles(LocationTiles currentTiles)
    {
        string finalWall = currentTiles.wall;
        string finalFloor = currentTiles.floor;
        if (!CheckWall(finalWall) || !CheckFloor(finalFloor)) return currentTiles;

        if(wall) finalWall = wall.nameID;
        if(floor) finalFloor = floor.nameID;

        return new LocationTiles(finalWall, finalFloor);
    }
}