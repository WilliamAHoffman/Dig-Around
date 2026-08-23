using UnityEngine;
using UnityEngine.Tilemaps;
public enum TileMapLayer
{
    Ground,
    Structure,
    Overlay
}

public sealed class BloxelContext
{
    public Tilemap Map { get; }
    public Vector2Int Position { get; }
    public TileMapLayer Layer { get; }
    public GameObject Actor { get; }

    public BloxelContext(
        Tilemap map,
        Vector2Int position,
        TileMapLayer layer,
        GameObject actor)
    {
        Map = map;
        Position = position;
        Layer = layer;
        Actor = actor;
    }
}