using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "World Generation/Generation Rule")]
public class GenerationRule : ScriptableObject
{
    [Header("Placement Requirements")]
    public List<TileData> requiredFloor;
    public List<TileData> requiredWall;

    [Header("Replacement")]
    public TileData newFloor;
    public TileData newWall;

    public GenerationResult Apply(GenerationResult result)
    {
        if ((requiredFloor.Count == 0 || requiredFloor.Contains(result.floor)) && newFloor)
            result.floor = newFloor;

        if ((requiredWall.Count == 0 || requiredWall.Contains(result.wall)) && newWall)
            result.wall = newWall;

        return result;
    }
}