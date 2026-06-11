using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldDataRegistry : MonoBehaviour
{
    public static WorldDataRegistry Instance { get; private set; }

    [Header("Tile Data")]
    [SerializeField] private List<WallData> wallDataList;
    [SerializeField] private List<FloorData> floorDataList;

    private Dictionary<string, WallData> wallDataByID;
    private Dictionary<string, FloorData> floorDataByID;

    [Header("Biome Data")]
    [SerializeField] private List<Biome> biomeDataList;

    private Dictionary<string, Biome> biomeDataByID;

    [Header("Fallback IDs")]
    [SerializeField] private string airWallID = "air";
    [SerializeField] private string defaultFloorID = "air";

    //[Header("Feature Data")]
    //[SerializeField] 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        wallDataByID = BuildDataLookup(wallDataList);
        floorDataByID = BuildDataLookup(floorDataList);
        biomeDataByID = BuildDataLookup(biomeDataList);
    }

    private Dictionary<string, T> BuildDataLookup<T>(List<T> datas) where T : ObjectID
    {
        Dictionary<string, T> dataByID = new Dictionary<string, T>();

        foreach (T data in datas)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.nameID))
                continue;

            if (dataByID.ContainsKey(data.nameID))
            {
                Debug.LogError($"Duplicate data nameID found: {data.nameID}", this);
                continue;
            }

            dataByID.Add(data.nameID, data);
        }

        return dataByID;
    }

    public WallData GetWallData(string nameID)
    {
        if (string.IsNullOrWhiteSpace(nameID))
            return null;

        if (wallDataByID.TryGetValue(nameID, out WallData wallData))
            return wallData;

        Debug.LogWarning($"No WallData found with nameID: {nameID}", this);
        return null;
    }

    public FloorData GetFloorData(string nameID)
    {
        if (string.IsNullOrWhiteSpace(nameID))
            return null;

        if (floorDataByID.TryGetValue(nameID, out FloorData floorData))
            return floorData;

        Debug.LogWarning($"No FloorData found with nameID: {nameID}", this);
        return null;
    }

    public Biome GetBiomeData(string nameID)
    {
        if (string.IsNullOrWhiteSpace(nameID))
            return null;

        if (biomeDataByID.TryGetValue(nameID, out Biome biome))
            return biome;

        Debug.LogWarning($"No Biome found with nameID: {nameID}", this);
        return null;
    }

    public WorldWallTile GetWorldWall(string nameID)
    {
        WallData wallData = GetWallData(nameID);

        if (wallData == null)
        {
            WallData fallbackWallData = GetWallData(airWallID);
            return new WorldWallTile(fallbackWallData.nameID, fallbackWallData.maxHealth);
        }

        return new WorldWallTile(wallData.nameID, wallData.maxHealth);
    }

    public WorldFloorTile GetWorldFloor(string nameID)
    {
        FloorData floorData = GetFloorData(nameID);

        if (floorData == null)
        {
            FloorData fallbackFloorData = GetFloorData(defaultFloorID);
            return new WorldFloorTile(fallbackFloorData.nameID, fallbackFloorData.maxHealth);
        }

        return new WorldFloorTile(floorData.nameID, floorData.maxHealth);
    }
}