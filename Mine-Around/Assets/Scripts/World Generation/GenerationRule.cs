using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "World Generation/Generation Rule")]
public class GenerationRule : ScriptableObject
{
    [Header("Placement Requirements")]
    public TileData requiredFloor;
    public TileData requiredWall;

    [Header("Replacement")]
    public TileData newFloor;
    public TileData newWall;

    public GenerationResult Apply(GenerationResult result)
    {
        if ((requiredFloor == null || result.floor == requiredFloor) && newFloor)
            result.floor = newFloor;

        if ((requiredWall == null || result.wall == requiredWall) && newWall)
            result.wall = newWall;

        return result;
    }
}