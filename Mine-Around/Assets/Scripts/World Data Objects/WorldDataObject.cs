using UnityEngine;

public abstract class WorldDataObject : ScriptableObject
{
    [Header("Identity")]
    public string nameID;
    public string displayName;
    public RNGOverride rngOverride;

    [TextArea(3, 5)] 
    [SerializeField] private string developerNotes;

    public string GetObjectSeed()
    {
        string salt = nameID;
        if(rngOverride != null)
        {
            salt = rngOverride.salt;
        }

        return salt;
    }
}