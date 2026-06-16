using UnityEngine;

public abstract class WorldDataObject : ScriptableObject
{
    [Header("Identity")]
    public string nameID;
    public string displayName;
}