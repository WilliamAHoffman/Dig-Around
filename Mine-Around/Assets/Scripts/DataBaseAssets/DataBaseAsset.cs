using System;
using UnityEngine;

public abstract class DatabaseAsset : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string nameID;
    [SerializeField] private string displayName;
    [SerializeField] private BasicSeed rngOverride;
    public abstract string pathName{ get;}
    public int ID { get; set; }

    /// <summary>
    /// Permanent human-readable identifier used by the database.
    ///
    /// Treat this as stable once the asset is used in released content,
    /// especially because it may affect deterministic randomness.
    /// </summary>
    public string NameID => pathName + nameID;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(displayName)
            ? nameID
            : displayName;


    /// <summary>
    /// Returns the deterministic seed associated with this asset.
    /// Uses the configured override when present, otherwise derives
    /// a stable seed from NameID.
    /// </summary>
    public int GetObjectSeed()
    {
        if (rngOverride != null)
        {
            return rngOverride.seed;
        }

        return GameRandomness.StableHash(nameID);
    }

    /// <summary>
    /// Called by GameDatabase after this asset has been successfully
    /// registered.
    ///
    /// Implementations must not modify identity or registry ownership.
    /// </summary>
    public virtual void Initialize()
    {
    }
}