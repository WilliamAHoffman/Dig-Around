using UnityEngine;
using UnityEngine.Tilemaps;

public class LocationTiles
{
    public string wall;
    public string floor;

    public LocationTiles(string wall, string floor)
    {
        this.wall = wall;
        this.floor = floor;
    }
}
