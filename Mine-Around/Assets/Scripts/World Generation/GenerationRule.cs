using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "World Generation/Generation Rule")]
public class GenerationRule : ScriptableObject
{
    public List<TileDataAsset> requiredFloor = new();
    public List<TileDataAsset> requiredWall = new();
    public List<TileDataAsset> excludedFloor = new();
    public List<TileDataAsset> excludedWall = new();
    private List<int> requiredFloorID = null;
    private List<int> requiredWallID = null;
    private List<int> excludedFloorID = null;
    private List<int> excludedWallID = null;
    private bool Initialized = false;

    [Header("Replacement")]
    public TileDataAsset newFloor;
    public TileDataAsset newWall;

    private void CheckListInitialized()
    {
        if(Initialized) return;

        requiredFloor.ForEach(n => requiredFloorID.Add(n.ID));
        requiredWall.ForEach(n => requiredWallID.Add(n.ID));
        excludedFloor.ForEach(n => excludedFloorID.Add(n.ID));
        excludedWall.ForEach(n => excludedWallID.Add(n.ID));
        Initialized = true;
    }
    public MapCell Apply(MapCell result)
    {
        CheckListInitialized();

        if (CanReplaceFloor(result.FloorID) && newFloor)
        {
            result.FloorID = newFloor.ID;
        }

        if (CanReplaceWall(result.WallID) && newWall)
        {
            result.WallID = newWall.ID;
        }

        return result;
    }

    private bool CanReplaceFloor(int current)
    {
        bool required =
            requiredFloorID.Count == 0 ||
            requiredFloorID.Contains(current);

        bool excluded =
            excludedFloorID.Contains(current);

        return required &&
               !excluded &&
               newFloor != null;
    }

    private bool CanReplaceWall(int current)
    {
        bool required =
            requiredWallID.Count == 0 ||
            requiredWallID.Contains(current);

        bool excluded =
            excludedWallID.Contains(current);

        return required &&
               !excluded &&
               newFloor != null;
    }
}