using System;
using UnityEngine;

public abstract class DatabaseAsset : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string nameID;
    [SerializeField] private int id = -1;
    [SerializeField] private string displayName;
    [SerializeField] private BasicSeed rngOverride;

    [TextArea(3, 5)]
    [SerializeField] private string developerNotes;

    /// <summary>
    /// Permanent human-readable identifier used by the database.
    ///
    /// Treat this as stable once the asset is used in released content,
    /// especially because it may affect deterministic randomness.
    /// </summary>
    public string NameID => nameID;

    /// <summary>
    /// Numeric registry identifier.
    ///
    /// Treat this as permanent if it is stored in save data.
    /// </summary>
    public int ID => id;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(displayName)
            ? nameID
            : displayName;

    /// <summary>
    /// Defines which database registry owns this asset's
    /// integer and string identifiers.
    ///
    /// Multiple concrete asset types may share the same registry.
    /// </summary>
    public abstract Type RegistryType { get; }

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