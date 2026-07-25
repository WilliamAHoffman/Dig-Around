using UnityEngine;
using UnityEngine.Tilemaps;
public enum TileLayer
{
    Ground,
        Structure,
    Overlay
}

public sealed class TileContext
{
    public Tilemap Map { get; }
    public Vector2Int Position { get; }
    public TileLayer Layer { get; }
    public GameObject Actor { get; }

    public TileContext(
        Tilemap map,
        Vector2Int position,
        TileLayer layer,
        GameObject actor)
    {
        Map = map;
        Position = position;
        Layer = layer;
        Actor = actor;
    }
}