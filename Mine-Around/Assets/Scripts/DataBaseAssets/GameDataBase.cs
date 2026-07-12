using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldDataObjectDataBase", menuName = "Storage/WorldDataObjectDataBase")]
public class GameDatabase : ScriptableObject
{

    [Header("Registered Objects")]
    [SerializeField] private List<DatabaseAsset> allAssets = new List<DatabaseAsset>();

    [Header("Default Object")]
    [SerializeField] private List<DatabaseAsset> defaultAssets = new List<DatabaseAsset>();

    private readonly Dictionary<int, DatabaseAsset> idLookup = new Dictionary<int, DatabaseAsset>();
    private readonly Dictionary<string, int> nameLookup = new Dictionary<string, int>();
    private readonly Dictionary<Type, List<DatabaseAsset>> typeLookup = new Dictionary<Type, List<DatabaseAsset>>();
    private readonly Dictionary<Type, DatabaseAsset> defaultLookup = new Dictionary<Type, DatabaseAsset>();

    private bool initialized;

    /// <summary>
    /// Rebuilds all lookup tables from the configured asset list.
    /// Safe to call manually after changing the asset list at runtime or from editor tools.
    /// </summary>
    public void Initialize()
    {
        idLookup.Clear();
        nameLookup.Clear();
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

        for(int i = 0; i < allAssets.Count; i++)
        {
            DatabaseAsset asset = allAssets[i];
            
            if (asset == null)
                continue;

            if (string.IsNullOrWhiteSpace(asset.nameID))
            {
                Debug.LogError($"Asset '{asset.name}' has an empty nameID.", this);
                continue;
            }

            if (nameLookup.ContainsKey(asset.nameID))
            {
                Debug.LogError($"Duplicate WorldDataObject nameID found: '{asset.nameID}'. Asset '{asset.name}' was ignored.", this);
                continue;
            }
            asset.ID = i;
            asset.Initialize();

            nameLookup.Add(asset.nameID, i);
            idLookup.Add(i, asset);

            Type assetType = asset.GetType();

            if (!typeLookup.TryGetValue(assetType, out List<DatabaseAsset> assetsOfType))
            {
                assetsOfType = new List<DatabaseAsset>();
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
        if (defaultAssets == null)
            return;

        foreach (DatabaseAsset defaultAsset in defaultAssets)
        {

            if (defaultAsset == null)
            {
                Debug.LogError($"Default WorldDataObject '{defaultAsset}' could not be found in the registered asset list.", this);
                continue;
            }

            Type assetType = defaultAsset.GetType();

            if (defaultLookup.ContainsKey(defaultAsset.GetType()))
            {
                Debug.LogWarning($"Multiple default assets were registered for type {assetType.Name}. '{defaultAsset.nameID}' replaced '{defaultLookup[assetType].nameID}'.", this);
            }

            defaultLookup[defaultAsset.GetType()] = defaultAsset;
        }
    }

    /// <summary>
    /// Returns an inheritance-aware list of all registered assets assignable to T.
    /// For example, GetAllAssetsOfType<TileData>() will include TileData subclasses too.
    /// </summary>
    public List<T> GetAllAssetsOfType<T>() where T : DatabaseAsset
    {
        EnsureInitialized();

        List<T> result = new List<T>();

        if (allAssets == null)
            return result;

        foreach (DatabaseAsset asset in allAssets)
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
    public List<T> GetAllAssetsOfExactType<T>() where T : DatabaseAsset
    {
        EnsureInitialized();

        Type targetType = typeof(T);
        List<T> result = new List<T>();

        if (!typeLookup.TryGetValue(targetType, out List<DatabaseAsset> assets))
            return result;

        foreach (DatabaseAsset asset in assets)
        {
            if (asset is T typedAsset)
                result.Add(typedAsset);
        }

        return result;
    }

    public T GetDefaultAsset<T>() where T : DatabaseAsset
    {
        EnsureInitialized();

        Type targetType = typeof(T);

        if (defaultLookup.TryGetValue(targetType, out DatabaseAsset asset))
            return asset as T;

        Debug.LogWarning($"Default object of type {targetType.Name} could not be found.", this);
        return null;
    }

    public T GetAssetByID<T>(int id) where T : DatabaseAsset
    {
        EnsureInitialized();

        DatabaseAsset asset = GetAssetByIDInternal(id, logMissing: false);

        if (asset == null)
            return null;

        if (asset is T typedAsset)
            return typedAsset;

        Debug.LogWarning($"WorldDataObject '{id}' exists but is not of type {typeof(T).Name}. It is {asset.GetType().Name}.", this);
        return null;
    }

    public bool TryGetAssetByID<T>(int id, out T asset) where T : DatabaseAsset
    {
        asset = GetAssetByID<T>(id);
        return asset != null;
    }

    public bool HasAssetByName(string id)
    {
        EnsureInitialized();

        if (string.IsNullOrWhiteSpace(id))
            return false;

        return nameLookup.ContainsKey(id);
    }

    public bool HasAssetByID(int id)
    {
        EnsureInitialized();

        return idLookup.ContainsKey(id);
    }


    private DatabaseAsset GetAssetByIDInternal(int id, bool logMissing = true)
    {

        if (idLookup.TryGetValue(id, out DatabaseAsset asset))
            return asset;

        if (logMissing)
            Debug.LogWarning($"WorldDataObject '{id}' could not be found.", this);

        return null;
    }
}
