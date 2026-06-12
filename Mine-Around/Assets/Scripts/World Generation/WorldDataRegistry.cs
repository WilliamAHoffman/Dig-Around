using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    [Header("Feature Data")]
    [SerializeField] private List<Feature> featureDataList;
    private Dictionary<string, Feature> featureDataByID;

    [Header("Fallback IDs")]
    [SerializeField] private string defaultID = "empty";

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
        featureDataByID = BuildDataLookup(featureDataList);
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

    public Feature GetFeatureData(string nameID)
    {
        if (string.IsNullOrWhiteSpace(nameID))
            return null;

        if (featureDataByID.TryGetValue(nameID, out Feature feature))
            return feature;

        Debug.LogWarning($"No Feature found with nameID: {nameID}", this);
        return null;
    }

    public string GetDefaultID()
    {
        return defaultID;
    }

    public WorldWallTile GetWorldWall(string nameID)
    {
        WallData wallData = GetWallData(nameID);

        return new WorldWallTile(wallData.nameID, wallData.maxHealth);
    }

    public WorldFloorTile GetWorldFloor(string nameID)
    {
        FloorData floorData = GetFloorData(nameID);

        return new WorldFloorTile(floorData.nameID, floorData.maxHealth);
    }
}