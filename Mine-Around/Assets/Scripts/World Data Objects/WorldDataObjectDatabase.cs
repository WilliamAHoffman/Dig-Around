using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldDataObjectDataBase", menuName = "Storage/WorldDataObjectDataBase")]
public class WorldDataObjectDataBase : ScriptableObject
{

    [Header("Registered Assets")]
    [SerializeField] private List<WorldDataObject> allAssets = new List<WorldDataObject>();

    [Header("Default Assets")]
    [Tooltip("List of WorldDataObject nameIDs used as defaults. Usually one default per WorldDataObject-derived type, such as the air TileData.")]
    [SerializeField] private List<WorldDataObject> defaultWorldDataObjects = new List<WorldDataObject>();

    private readonly Dictionary<string, WorldDataObject> idLookup = new Dictionary<string, WorldDataObject>();
    private readonly Dictionary<Type, List<WorldDataObject>> typeLookup = new Dictionary<Type, List<WorldDataObject>>();
    private readonly Dictionary<Type, WorldDataObject> defaultLookup = new Dictionary<Type, WorldDataObject>();

    private bool initialized;

    /// <summary>
    /// Rebuilds all lookup tables from the configured asset list.
    /// Safe to call manually after changing the asset list at runtime or from editor tools.
    /// </summary>
    public void Initialize()
    {
        idLookup.Clear();
        typeLookup.Clear();
        defaultLookup.Clear();

        BuildAssetLookups();
        BuildDefaultLookups();

        initialized = true;
    }

    private void EnsureInitialized()
    {
        if (!initialized)
            Initialize();
    }

    private void BuildAssetLookups()
    {
        if (allAssets == null)
            return;

        foreach (WorldDataObject asset in allAssets)
        {
            if (asset == null)
                continue;

            if (string.IsNullOrWhiteSpace(asset.nameID))
            {
                Debug.LogError($"Asset '{asset.name}' has an empty nameID.", this);
                continue;
            }

            if (idLookup.ContainsKey(asset.nameID))
            {
                Debug.LogError($"Duplicate WorldDataObject nameID found: '{asset.nameID}'. Asset '{asset.name}' was ignored.", this);
                continue;
            }

            idLookup.Add(asset.nameID, asset);

            Type assetType = asset.GetType();

            if (!typeLookup.TryGetValue(assetType, out List<WorldDataObject> assetsOfType))
            {
                assetsOfType = new List<WorldDataObject>();
                typeLookup.Add(assetType, assetsOfType);
            }

            if (string.IsNullOrWhiteSpace(asset.displayName))
            {
                asset.displayName = asset.nameID;
            }

            assetsOfType.Add(asset);
        }
    }

    private void BuildDefaultLookups()
    {
        if (defaultWorldDataObjects == null)
            return;

        foreach (WorldDataObject defaultAsset in defaultWorldDataObjects)
        {

            if (defaultAsset == null)
            {
                Debug.LogError($"Default WorldDataObject '{defaultAsset}' could not be found in the registered asset list.", this);
                continue;
            }

            Type assetType = defaultAsset.GetType();

            if (defaultLookup.ContainsKey(assetType))
            {
                Debug.LogWarning($"Multiple default assets were registered for type {assetType.Name}. '{defaultAsset.nameID}' replaced '{defaultLookup[assetType].nameID}'.", this);
            }

            defaultLookup[assetType] = defaultAsset;
        }
    }

    /// <summary>
    /// Returns an inheritance-aware list of all registered assets assignable to T.
    /// For example, GetAllAssetsOfType<TileData>() will include TileData subclasses too.
    /// </summary>
    public List<T> GetAllAssetsOfType<T>() where T : WorldDataObject
    {
        EnsureInitialized();

        List<T> result = new List<T>();

        if (allAssets == null)
            return result;

        foreach (WorldDataObject asset in allAssets)
        {
            if (asset is T typedAsset)
                result.Add(typedAsset);
        }

        return result;
    }

    /// <summary>
    /// Faster exact-type lookup. This only returns assets whose runtime type is exactly T.
    /// Use GetAllAssetsOfType<T>() if subclasses should be included.
    /// </summary>
    public List<T> GetAllAssetsOfExactType<T>() where T : WorldDataObject
    {
        EnsureInitialized();

        Type targetType = typeof(T);
        List<T> result = new List<T>();

        if (!typeLookup.TryGetValue(targetType, out List<WorldDataObject> assets))
            return result;

        foreach (WorldDataObject asset in assets)
        {
            if (asset is T typedAsset)
                result.Add(typedAsset);
        }

        return result;
    }

    public T GetDefaultAsset<T>() where T : WorldDataObject
    {
        EnsureInitialized();

        Type targetType = typeof(T);

        if (defaultLookup.TryGetValue(targetType, out WorldDataObject asset))
            return asset as T;

        Debug.LogWarning($"Default object of type {targetType.Name} could not be found.", this);
        return null;
    }

    public T GetAssetByID<T>(string id) where T : WorldDataObject
    {
        EnsureInitialized();

        WorldDataObject asset = GetAssetByIDInternal(id, logMissing: false);

        if (asset == null)
            return null;

        if (asset is T typedAsset)
            return typedAsset;

        Debug.LogWarning($"WorldDataObject '{id}' exists but is not of type {typeof(T).Name}. It is {asset.GetType().Name}.", this);
        return null;
    }

    public bool TryGetAssetByID<T>(string id, out T asset) where T : WorldDataObject
    {
        asset = GetAssetByID<T>(id);
        return asset != null;
    }

    public bool HasAsset(string id)
    {
        EnsureInitialized();

        if (string.IsNullOrWhiteSpace(id))
            return false;

        return idLookup.ContainsKey(id);
    }

    private WorldDataObject GetAssetByIDInternal(string id, bool logMissing = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        if (idLookup.TryGetValue(id, out WorldDataObject asset))
            return asset;

        if (logMissing)
            Debug.LogWarning($"WorldDataObject '{id}' could not be found.", this);

        return null;
    }

    public TileData GetTileData(string nameID)
    {
        TileData tileData = GetAssetByID<TileData>(nameID);

        if (tileData != null)
            return tileData;

        TileData defaultTile = GetDefaultAsset<TileData>();

        if (defaultTile == null)
            Debug.LogError($"TileData '{nameID}' could not be found and no default TileData is registered.", this);

        return defaultTile;
    }

    public WorldTile GetWorldTile(string nameID)
    {
        TileData tileData = GetTileData(nameID);
        return tileData != null ? tileData.WorldTile() : null;
    }

    public string GetDefaultID<T>() where T : WorldDataObject
    {
        T defaultAsset = GetDefaultAsset<T>();
        return defaultAsset != null ? defaultAsset.nameID : string.Empty;
    }
}
