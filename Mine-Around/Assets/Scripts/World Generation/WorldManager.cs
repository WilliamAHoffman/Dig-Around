using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    [Header("World Settings")]
    [SerializeField] private int chunkSize = 16;
    public int ChunkSize => chunkSize;

    [Header("Tile Data")]
    [SerializeField] private List<WallData> wallDataList;
    [SerializeField] private List<FloorData> floorDataList;

    private Dictionary<string, WallData> wallDataByID;
    private Dictionary<string, FloorData> floorDataByID;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildTileDataLookups();
    }

    private void BuildTileDataLookups()
    {
        wallDataByID = new Dictionary<string, WallData>();
        floorDataByID = new Dictionary<string, FloorData>();

        foreach (WallData wallData in wallDataList)
        {
            if (wallData == null || string.IsNullOrWhiteSpace(wallData.nameID))
                continue;

            if (wallDataByID.ContainsKey(wallData.nameID))
            {
                Debug.LogError($"Duplicate wall nameID found: {wallData.nameID}");
                continue;
            }

            wallDataByID.Add(wallData.nameID, wallData);
        }

        foreach (FloorData floorData in floorDataList)
        {
            if (floorData == null || string.IsNullOrWhiteSpace(floorData.nameID))
                continue;

            if (floorDataByID.ContainsKey(floorData.nameID))
            {
                Debug.LogError($"Duplicate floor nameID found: {floorData.nameID}");
                continue;
            }

            floorDataByID.Add(floorData.nameID, floorData);
        }
    }

    public WallData GetWallData(string nameID)
    {
        if (string.IsNullOrWhiteSpace(nameID))
            return null;

        if (wallDataByID.TryGetValue(nameID, out WallData wallData))
            return wallData;

        Debug.LogWarning($"No WallData found with nameID: {nameID}");
        return null;
    }

    public FloorData GetFloorData(string nameID)
    {
        if (string.IsNullOrWhiteSpace(nameID))
            return null;

        if (floorDataByID.TryGetValue(nameID, out FloorData floorData))
            return floorData;

        Debug.LogWarning($"No FloorData found with nameID: {nameID}");
        return null;
    }
}