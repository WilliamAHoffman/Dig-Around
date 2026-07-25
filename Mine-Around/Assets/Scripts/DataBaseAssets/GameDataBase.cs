using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "GameDatabase",
    menuName = "Storage/Game Database"
)]
public sealed class GameDatabase : ScriptableObject
{
    [Header("Registered Assets")]
    [SerializeField]
    private List<DatabaseAsset> allAssets = new();

    [Header("Default Assets")]
    [SerializeField]
    private List<DatabaseAsset> defaultAssets = new();

    /*
     * Registry category
     *     ↓
     * Numeric ID → asset
     *
     * TileData:
     *     1 → Grass
     *
     * BiomeData:
     *     1 → Plains
     */
    private readonly Dictionary<
        Type,
        Dictionary<int, DatabaseAsset>
    > idLookups = new();

    /*
     * Registry category
     *     ↓
     * String ID → asset
     */
    private readonly Dictionary<
        Type,
        Dictionary<string, DatabaseAsset>
    > nameLookups = new();

    /*
     * Exact runtime type
     *     ↓
     * All assets whose concrete C# type exactly matches
     */
    private readonly Dictionary<
        Type,
        List<DatabaseAsset>
    > exactTypeLookups = new();

    /*
     * Registry category
     *     ↓
     * Default asset for that category
     */
    private readonly Dictionary<
        Type,
        DatabaseAsset
    > defaultLookups = new();

    private bool initialized;

    private void OnEnable()
    {
        // Runtime dictionaries are not serialized by Unity.
        initialized = false;
    }

    /// <summary>
    /// Rebuilds all runtime lookup tables from the serialized asset lists.
    /// </summary>
    public void Initialize()
    {
        idLookups.Clear();
        nameLookups.Clear();
        exactTypeLookups.Clear();
        defaultLookups.Clear();

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

        for (int index = 0; index < allAssets.Count; index++)
        {
            DatabaseAsset asset = allAssets[index];

            if (asset == null)
            {
                Debug.LogWarning(
                    $"Registered asset entry {index} is null.",
                    this
                );

                continue;
            }

            if (!ValidateAssetIdentity(asset))
                continue;

            Type registryType = asset.RegistryType;

            if (!ValidateRegistryType(asset, registryType))
                continue;

            Dictionary<int, DatabaseAsset> idRegistry =
                GetOrCreateIDRegistry(registryType);

            Dictionary<string, DatabaseAsset> nameRegistry =
                GetOrCreateNameRegistry(registryType);

            if (idRegistry.TryGetValue(
                    asset.ID,
                    out DatabaseAsset duplicateIDAsset))
            {
                Debug.LogError(
                    $"Duplicate ID {asset.ID} in registry " +
                    $"'{registryType.Name}'. " +
                    $"Asset '{asset.name}' conflicts with " +
                    $"'{duplicateIDAsset.name}'.",
                    asset
                );

                continue;
            }

            if (nameRegistry.TryGetValue(
                    asset.NameID,
                    out DatabaseAsset duplicateNameAsset))
            {
                Debug.LogError(
                    $"Duplicate name ID '{asset.NameID}' in registry " +
                    $"'{registryType.Name}'. " +
                    $"Asset '{asset.name}' conflicts with " +
                    $"'{duplicateNameAsset.name}'.",
                    asset
                );

                continue;
            }

            idRegistry.Add(asset.ID, asset);
            nameRegistry.Add(asset.NameID, asset);

            AddToExactTypeLookup(asset);

            asset.Initialize();
        }
    }

    private bool ValidateAssetIdentity(DatabaseAsset asset)
    {
        if (asset.ID < 0)
        {
            Debug.LogError(
                $"Asset '{asset.name}' has invalid ID {asset.ID}. " +
                "IDs must be zero or greater.",
                asset
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(asset.NameID))
        {
            Debug.LogError(
                $"Asset '{asset.name}' has an empty name ID.",
                asset
            );

            return false;
        }

        return true;
    }

    private bool ValidateRegistryType(
        DatabaseAsset asset,
        Type registryType)
    {
        if (registryType == null)
        {
            Debug.LogError(
                $"Asset '{asset.name}' returned a null registry type.",
                asset
            );

            return false;
        }

        if (!typeof(DatabaseAsset).IsAssignableFrom(registryType))
        {
            Debug.LogError(
                $"Registry type '{registryType.Name}' on asset " +
                $"'{asset.name}' does not derive from DatabaseAsset.",
                asset
            );

            return false;
        }

        if (!registryType.IsAssignableFrom(asset.GetType()))
        {
            Debug.LogError(
                $"Asset '{asset.name}' is of type " +
                $"'{asset.GetType().Name}', which cannot belong to " +
                $"registry '{registryType.Name}'.",
                asset
            );

            return false;
        }

        return true;
    }

    private Dictionary<int, DatabaseAsset> GetOrCreateIDRegistry(
        Type registryType)
    {
        if (!idLookups.TryGetValue(
                registryType,
                out Dictionary<int, DatabaseAsset> registry))
        {
            registry = new Dictionary<int, DatabaseAsset>();
            idLookups.Add(registryType, registry);
        }

        return registry;
    }

    private Dictionary<string, DatabaseAsset> GetOrCreateNameRegistry(
        Type registryType)
    {
        if (!nameLookups.TryGetValue(
                registryType,
                out Dictionary<string, DatabaseAsset> registry))
        {
            registry = new Dictionary<string, DatabaseAsset>(
                StringComparer.Ordinal
            );

            nameLookups.Add(registryType, registry);
        }

        return registry;
    }

    private void AddToExactTypeLookup(DatabaseAsset asset)
    {
        Type exactType = asset.GetType();

        if (!exactTypeLookups.TryGetValue(
                exactType,
                out List<DatabaseAsset> assets))
        {
            assets = new List<DatabaseAsset>();
            exactTypeLookups.Add(exactType, assets);
        }

        assets.Add(asset);
    }

    private void BuildDefaultLookups()
    {
        if (defaultAssets == null)
            return;

        for (int index = 0; index < defaultAssets.Count; index++)
        {
            DatabaseAsset defaultAsset = defaultAssets[index];

            if (defaultAsset == null)
            {
                Debug.LogError(
                    $"Default asset entry {index} is null.",
                    this
                );

                continue;
            }

            Type registryType = defaultAsset.RegistryType;

            if (!ValidateRegistryType(defaultAsset, registryType))
                continue;

            if (!IsRegistered(defaultAsset))
            {
                Debug.LogError(
                    $"Default asset '{defaultAsset.name}' is not " +
                    $"registered in the '{registryType.Name}' registry.",
                    defaultAsset
                );

                continue;
            }

            if (defaultLookups.TryGetValue(
                    registryType,
                    out DatabaseAsset previousDefault))
            {
                Debug.LogWarning(
                    $"Multiple defaults were assigned to registry " +
                    $"'{registryType.Name}'. " +
                    $"'{defaultAsset.NameID}' replaced " +
                    $"'{previousDefault.NameID}'.",
                    this
                );
            }

            defaultLookups[registryType] = defaultAsset;
        }
    }

    private bool IsRegistered(DatabaseAsset asset)
    {
        Type registryType = asset.RegistryType;

        if (!idLookups.TryGetValue(
                registryType,
                out Dictionary<int, DatabaseAsset> registry))
        {
            return false;
        }

        return registry.TryGetValue(asset.ID, out DatabaseAsset found) &&
               found == asset;
    }

    /// <summary>
    /// Returns an asset from the registry category represented by T.
    ///
    /// Example:
    /// GetAssetByID&lt;TileData&gt;(1)
    /// GetAssetByID&lt;BiomeData&gt;(1)
    /// </summary>
    public T GetAssetByID<T>(int id)
        where T : DatabaseAsset
    {
        if (TryGetAssetByID(id, out T asset))
            return asset;

        Debug.LogWarning(
            $"Asset ID {id} could not be found in registry " +
            $"'{typeof(T).Name}'.",
            this
        );

        return null;
    }

    public bool TryGetAssetByID<T>(int id, out T asset)
        where T : DatabaseAsset
    {
        EnsureInitialized();

        asset = null;

        Type registryType = typeof(T);

        if (!idLookups.TryGetValue(
                registryType,
                out Dictionary<int, DatabaseAsset> registry))
        {
            return false;
        }

        if (!registry.TryGetValue(id, out DatabaseAsset found))
            return false;

        asset = found as T;
        return asset != null;
    }

    /// <summary>
    /// Performs a lookup when the registry category is only known at runtime.
    /// </summary>
    public bool TryGetAssetByID(
        Type registryType,
        int id,
        out DatabaseAsset asset)
    {
        EnsureInitialized();

        asset = null;

        if (registryType == null)
            return false;

        if (!idLookups.TryGetValue(
                registryType,
                out Dictionary<int, DatabaseAsset> registry))
        {
            return false;
        }

        return registry.TryGetValue(id, out asset);
    }

    public T GetAssetByName<T>(string nameID)
        where T : DatabaseAsset
    {
        if (TryGetAssetByName(nameID, out T asset))
            return asset;

        Debug.LogWarning(
            $"Asset '{nameID}' could not be found in registry " +
            $"'{typeof(T).Name}'.",
            this
        );

        return null;
    }

    public bool TryGetAssetByName<T>(
        string nameID,
        out T asset)
        where T : DatabaseAsset
    {
        EnsureInitialized();

        asset = null;

        if (string.IsNullOrWhiteSpace(nameID))
            return false;

        Type registryType = typeof(T);

        if (!nameLookups.TryGetValue(
                registryType,
                out Dictionary<string, DatabaseAsset> registry))
        {
            return false;
        }

        if (!registry.TryGetValue(nameID, out DatabaseAsset found))
            return false;

        asset = found as T;
        return asset != null;
    }

    public bool TryGetAssetByName(
        Type registryType,
        string nameID,
        out DatabaseAsset asset)
    {
        EnsureInitialized();

        asset = null;

        if (registryType == null ||
            string.IsNullOrWhiteSpace(nameID))
        {
            return false;
        }

        if (!nameLookups.TryGetValue(
                registryType,
                out Dictionary<string, DatabaseAsset> registry))
        {
            return false;
        }

        return registry.TryGetValue(nameID, out asset);
    }

    public bool HasAssetByID<T>(int id)
        where T : DatabaseAsset
    {
        EnsureInitialized();

        return idLookups.TryGetValue(
                   typeof(T),
                   out Dictionary<int, DatabaseAsset> registry) &&
               registry.ContainsKey(id);
    }

    public bool HasAssetByName<T>(string nameID)
        where T : DatabaseAsset
    {
        EnsureInitialized();

        if (string.IsNullOrWhiteSpace(nameID))
            return false;

        return nameLookups.TryGetValue(
                   typeof(T),
                   out Dictionary<string, DatabaseAsset> registry) &&
               registry.ContainsKey(nameID);
    }

    /// <summary>
    /// Returns every asset registered under category T.
    /// Subclasses are included because they share T's registry.
    /// </summary>
    public List<T> GetAllAssetsOfType<T>()
        where T : DatabaseAsset
    {
        EnsureInitialized();

        List<T> result = new();

        if (!idLookups.TryGetValue(
                typeof(T),
                out Dictionary<int, DatabaseAsset> registry))
        {
            return result;
        }

        foreach (DatabaseAsset asset in registry.Values)
        {
            if (asset is T typedAsset)
                result.Add(typedAsset);
        }

        return result;
    }

    /// <summary>
    /// Returns assets whose concrete runtime type is exactly T.
    /// </summary>
    public List<T> GetAllAssetsOfExactType<T>()
        where T : DatabaseAsset
    {
        EnsureInitialized();

        List<T> result = new();

        if (!exactTypeLookups.TryGetValue(
                typeof(T),
                out List<DatabaseAsset> assets))
        {
            return result;
        }

        foreach (DatabaseAsset asset in assets)
            result.Add((T)asset);

        return result;
    }

    /// <summary>
    /// Returns the default asset for registry category T.
    /// </summary>
    public T GetDefaultAsset<T>()
        where T : DatabaseAsset
    {
        EnsureInitialized();

        Type registryType = typeof(T);

        if (defaultLookups.TryGetValue(
                registryType,
                out DatabaseAsset asset))
        {
            return asset as T;
        }

        Debug.LogWarning(
            $"No default asset is configured for registry " +
            $"'{registryType.Name}'.",
            this
        );

        return null;
    }
}