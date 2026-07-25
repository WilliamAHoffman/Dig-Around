
public struct GenerationResult
{
    public TileDataAsset floor;
    public TileDataAsset wall;
    public GenerationResult(TileDataAsset floor, TileDataAsset wall)
    {
        this.floor = floor;
        this.wall = wall;
    }

    public MapCell MapCellValues()
    {
        return new MapCell(floor.ID, wall.ID);
    }
}