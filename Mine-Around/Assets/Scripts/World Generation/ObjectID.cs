using UnityEngine;

public enum ObjectIDType
{
    Tile,
    Feature,
    Biome,
    GenerationRule,
    Noise
}

public abstract class ObjectID : ScriptableObject
{
    [Header("Identity")]
    public string nameID;
    public string displayName;

    public abstract ObjectIDType Type { get; }
}