
public struct GenerationResult
{
    public TileData floor;
    public TileData wall;
    public GenerationResult(TileData floor, TileData wall)
    {
        this.floor = floor;
        this.wall = wall;
    }

    public LocationTiles LocationTiles()
    {
        return new LocationTiles(wall.nameID, floor.nameID);
    }
}