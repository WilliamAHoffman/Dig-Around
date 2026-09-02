using UnityEngine;

public struct GenerationContext
{
    public Vector2Int Position;
    public MapCell Cell;
    public int Seed;

    public GenerationContext(Vector2Int position, MapCell cell, int seed)
    {
        Position = position;
        Cell = cell;
        Seed = seed;
    }
}
