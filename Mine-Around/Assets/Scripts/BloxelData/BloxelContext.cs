using UnityEngine;
using UnityEngine.Tilemaps;
public enum BloxelLayer
{
    Floor,
    Wall,
    Object
}

public sealed class BloxelContext
{
    public Tilemap Map { get; }
    public Vector2Int Position { get; }
    public BloxelLayer Layer { get; }
    public GameObject Actor { get; }

    public BloxelContext(
        Tilemap map,
        Vector2Int position,
        BloxelLayer layer,
        GameObject actor)
    {
        Map = map;
        Position = position;
        Layer = layer;
        Actor = actor;
    }
}