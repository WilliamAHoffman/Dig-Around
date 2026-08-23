using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type-safe database for one DatabaseAsset family.
///
/// Unity cannot create an asset from an open generic type. Create a concrete
/// subclass for each database, for example:
///
/// [CreateAssetMenu(fileName = "BlockDatabase", menuName = "Storage/Block Database")]
/// public sealed class BlockDatabase : GameDatabase&lt;BlockDataAsset&gt; { }
/// </summary>
public abstract class GameDatabase<TAsset> : ScriptableObject
    where TAsset : DatabaseAsset
{
    [Header("Registered Assets")]
    [SerializeField] private List<TAsset> assets = new();

    [Header("Default Asset")]
    [SerializeField] private TAsset defaultAsset;

    private readonly Dictionary<int, TAsset> assetsById = new();
    private readonly Dictionary<string, TAsset> assetsByName = new(StringComparer.Ordinal);

    private bool initialized;

    public IReadOnlyList<TAsset> Assets => assets;
    public TAsset DefaultAsset => defaultAsset;

    private void OnEnable()
    {
        // Dictionaries are runtime-only and must be rebuilt after a reload.
        initialized = false;
    }

    /// <summary>
    /// Rebuilds the runtime lookup tables. This does not initialize the
    /// individual assets, so rebuilding the index has no asset-side effects.
    /// </summary>
    public void RebuildLookups()
    {
        assetsById.Clear();
        assetsByName.Clear();

        for (int index = 0; index < assets.Count; index++)
        {
            TAsset asset = assets[index];

            if (asset == null)
            {
                Debug.LogError($"Asset entry {index} is null.", this);
                continue;
            }

            if (!ValidateIdentity(asset))
                continue;

            if (assetsById.TryGetValue(asset.ID, out TAsset duplicateId))
            {
                Debug.LogError(
                    $"Duplicate ID {asset.ID}: '{asset.name}' conflicts " +
                    $"with '{duplicateId.name}'.",
                    asset);
                continue;
            }

            if (assetsByName.TryGetValue(asset.NameID, out TAsset duplicateName))
            {
                Debug.LogError(
                    $"Duplicate name ID '{asset.NameID}': '{asset.name}' " +
                    $"conflicts with '{duplicateName.name}'.",
                    asset);
                continue;
            }

            // Commit only after all validation passes.
            assetsById.Add(asset.ID, asset);
            assetsByName.Add(asset.NameID, asset);
        }

        ValidateDefault();
        initialized = true;
    }

    /// <summary>
    /// Initializes every successfully registered asset. Call once during
    /// application startup if DatabaseAsset.Initialize has runtime work.
    /// </summary>
    public void InitializeAssets()
    {
        EnsureInitialized();

        foreach (TAsset asset in assetsById.Values)
            asset.Initialize();
    }

    public bool TryGetById(int id, out TAsset asset)
    {
        EnsureInitialized();
        return assetsById.TryGetValue(id, out asset);
    }

    public TAsset GetByIdOrDefault(int id)
    {
        return TryGetById(id, out TAsset asset) ? asset : defaultAsset;
    }

    public bool TryGetByName(string nameId, out TAsset asset)
    {
        EnsureInitialized();

        if (string.IsNullOrWhiteSpace(nameId))
        {
            asset = null;
            return false;
        }

        return assetsByName.TryGetValue(nameId, out asset);
    }

    public TAsset GetByNameOrDefault(string nameId)
    {
        return TryGetByName(nameId, out TAsset asset) ? asset : defaultAsset;
    }

    public bool ContainsId(int id)
    {
        EnsureInitialized();
        return assetsById.ContainsKey(id);
    }

    public bool ContainsName(string nameId)
    {
        EnsureInitialized();
        return !string.IsNullOrWhiteSpace(nameId) &&
               assetsByName.ContainsKey(nameId);
    }

    private void EnsureInitialized()
    {
        if (!initialized)
            RebuildLookups();
    }

    private static bool ValidateIdentity(TAsset asset)
    {
        if (asset.ID < 0)
        {
            Debug.LogError(
                $"Asset '{asset.name}' has invalid ID {asset.ID}. " +
                "IDs must be zero or greater.",
                asset);
            return false;
        }

        if (string.IsNullOrWhiteSpace(asset.NameID))
        {
            Debug.LogError($"Asset '{asset.name}' has an empty name ID.", asset);
            return false;
        }

        return true;
    }

    private void ValidateDefault()
    {
        if (defaultAsset == null)
            return;

        if (!assetsById.TryGetValue(defaultAsset.ID, out TAsset registered) ||
            registered != defaultAsset)
        {
            Debug.LogError(
                $"Default asset '{defaultAsset.name}' is not successfully " +
                "registered in this database.",
                defaultAsset);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Rebuild lazily after Inspector changes. Avoid running asset runtime
        // initialization from editor validation.
        initialized = false;
    }
#endif
}
