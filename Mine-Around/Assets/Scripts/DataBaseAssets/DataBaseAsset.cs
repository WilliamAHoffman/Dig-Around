using UnityEngine;

public abstract class DatabaseAsset : ScriptableObject
{
    [Header("Identity")]
    public string nameID;
    public int ID;
    public string displayName;
    public BasicSeed rngOverride;

    [TextArea(3, 5)] 
    [SerializeField] private string developerNotes;

    public int GetObjectSeed()
    {
        int salt = GameRandomness.StableHash(nameID);
        if(rngOverride != null)
        {
            salt = rngOverride.seed;
        }

        return salt;
    }

    public virtual void Initialize()
    {
        return;
    }
}