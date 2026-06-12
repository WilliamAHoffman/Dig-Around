using System.Collections.Generic;
using UnityEngine;

public class WorldDataRegistry : MonoBehaviour
{
    public static WorldDataRegistry Instance { get; private set; }

    [Header("Tile Data")]
    [SerializeField] private List<TileData> TileDataList;
    [SerializeField] private TileData empty_tile;

    private Dictionary<string, TileData> tileDataByID;

    [Header("Biome Data")]
    [SerializeField] private List<Biome> biomeDataList;
    [SerializeField] private Biome empty_biome;
    private Dictionary<string, Biome> biomeDataByID;

    [Header("Feature Data")]
    [SerializeField] private List<Feature> featureDataList;
    private Dictionary<string, Feature> featureDataByID;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        tileDataByID = BuildDataLookup(TileDataList);
        biomeDataByID = BuildDataLookup(biomeDataList);
        featureDataByID = BuildDataLookup(featureDataList);
    }

    public void SetFeatureNoise(int seed)
    {
        foreach (Feature feature in featureDataList)
        {
            feature.SetNoise(seed);
        }
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

    public TileData GetTileData(string nameID)
    {
        if (string.IsNullOrWhiteSpace(nameID))
            return null;

        if (tileDataByID.TryGetValue(nameID, out TileData floorData))
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

    public WorldTile GetAirTile()
    {
        return empty_tile.WorldTile();
    }

    public bool IsAirTile(string nameID)
    {
        return empty_tile != null && nameID == empty_tile.nameID;
    }
    public Biome GetEmptyBiome()
    {
        return empty_biome;
    }

    public WorldTile GetWorldTile(string nameID)
    {
        TileData tileData = GetTileData(nameID);

        if (tileData == null)
        {
            Debug.LogWarning($"Falling back to empty tile for missing tile ID: {nameID}", this);
            return GetAirTile();
        }

        return tileData.WorldTile();
    }
}