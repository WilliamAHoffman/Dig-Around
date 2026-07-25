using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "World Generation/Generation Rule")]
public class GenerationRule : ScriptableObject
{
    [Header("Placement Requirements")]
    public List<TileDataAsset> requiredFloor;
    public List<TileDataAsset> requiredWall;
    [Header("Placement Exclusions")]
    public List<TileDataAsset> excludedFloor;
    public List<TileDataAsset> excludedWall;

    [Header("Replacement")]
    public TileDataAsset newFloor;
    public TileDataAsset newWall;

    public GenerationResult Apply(GenerationResult result)
    {
        if ((requiredFloor.Count == 0 || requiredFloor.Contains(result.floor)) && newFloor && !excludedFloor.Contains(result.floor))
            result.floor = newFloor;

        if ((requiredWall.Count == 0 || requiredWall.Contains(result.wall)) && newWall && !excludedWall.Contains(result.wall))
            result.wall = newWall;

        return result;
    }
}