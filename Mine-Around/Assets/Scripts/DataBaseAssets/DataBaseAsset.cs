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

    public string NameID => nameID;
    public int ID => id;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(displayName)
            ? nameID
            : displayName;

    /// <summary>
    /// Defines which registry owns this asset's integer and string IDs.
    /// </summary>
    public abstract Type RegistryType { get; }

    public int GetObjectSeed()
    {
        if (rngOverride != null)
            return rngOverride.seed;

        return GameRandomness.StableHash(nameID);
    }

    public virtual void Initialize()
    {
    }
}