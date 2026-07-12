//information containing each tile at a location
public struct LocationTiles
{
    public int wall;
    public int floor;

    public LocationTiles(int wall, int floor)
    {
        this.wall = wall;
        this.floor = floor;
    }
}
