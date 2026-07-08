using UnityEngine;

public abstract class WorldDataObject : ScriptableObject
{
    [Header("Identity")]
    public string nameID;
    public string displayName;
    public BasicSeed rngOverride;

    [TextArea(3, 5)] 
    [SerializeField] private string developerNotes;

    public int GetObjectSeed()
    {
        int salt = WorldDataObjectRandomness.StableHash(nameID);
        if(rngOverride != null)
        {
            salt = rngOverride.seed;
        }

        return salt;
    }
}