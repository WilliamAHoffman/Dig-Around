using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "GenerationRule",
    menuName = "World Generation/Generation Rule"
)]
public class GenerationRule : ScriptableObject
{
    #region Tile Restrictions

    [Header("Required Tiles")]
    [SerializeField]
    private List<BloxelBase> requiredFloor = new();

    [SerializeField]
    private List<BloxelBase> requiredWall = new();

    [Header("Excluded Tiles")]
    [SerializeField]
    private List<BloxelBase> excludedFloor = new();

    [SerializeField]
    private List<BloxelBase> excludedWall = new();

    #endregion

    #region Replacement

    [Header("Replacement")]
    [SerializeField]
    private BloxelBase newFloor;

    [SerializeField]
    private BloxelBase newWall;

    #endregion

    #region Cached IDs

    private readonly HashSet<int> requiredFloorIDs = new();
    private readonly HashSet<int> requiredWallIDs = new();

    private readonly HashSet<int> excludedFloorIDs = new();
    private readonly HashSet<int> excludedWallIDs = new();

    #endregion

    #region Lifecycle

    private void OnEnable()
    {
        RebuildCache();
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        RebuildCache();
    }

#endif

    #endregion

    #region Application

    public MapCell Apply(MapCell result)
    {
        if (newFloor != null &&
            CanReplaceFloor(result.FloorID))
        {
            result.FloorID = newFloor.ID;
        }

        if (newWall != null &&
            CanReplaceWall(result.WallID))
        {
            result.WallID = newWall.ID;
        }

        return result;
    }

    #endregion

    #region Replacement Rules

    private bool CanReplaceFloor(int currentTileID)
    {
        bool meetsRequiredCondition =
            requiredFloorIDs.Count == 0 ||
            requiredFloorIDs.Contains(currentTileID);

        bool isExcluded =
            excludedFloorIDs.Contains(currentTileID);

        return meetsRequiredCondition &&
               !isExcluded;
    }

    private bool CanReplaceWall(int currentTileID)
    {
        bool meetsRequiredCondition =
            requiredWallIDs.Count == 0 ||
            requiredWallIDs.Contains(currentTileID);

        bool isExcluded =
            excludedWallIDs.Contains(currentTileID);

        return meetsRequiredCondition &&
               !isExcluded;
    }

    #endregion

    #region Cache

    private void RebuildCache()
    {
        requiredFloorIDs.Clear();
        requiredWallIDs.Clear();

        excludedFloorIDs.Clear();
        excludedWallIDs.Clear();

        AddTileIDs(
            requiredFloor,
            requiredFloorIDs
        );

        AddTileIDs(
            requiredWall,
            requiredWallIDs
        );

        AddTileIDs(
            excludedFloor,
            excludedFloorIDs
        );

        AddTileIDs(
            excludedWall,
            excludedWallIDs
        );
    }

    private static void AddTileIDs(
        List<BloxelBase> tiles,
        HashSet<int> destination)
    {
        if (tiles == null)
        {
            return;
        }

        foreach (BloxelBase tile in tiles)
        {
            if (tile == null)
            {
                continue;
            }

            destination.Add(tile.ID);
        }
    }

    #endregion
}