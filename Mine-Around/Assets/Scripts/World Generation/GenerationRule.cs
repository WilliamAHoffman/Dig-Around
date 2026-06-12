using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "Scriptable Objects/GenerationRule")]
public class GenerationRule : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.GenerationRule;
    public TileData tile;
    public List<TileData> replaces;

    public bool Replaces(string replacing)
    {
        if(replaces.Count == 0) return true;
        foreach(TileData tile in replaces)
        {
            if(tile.nameID == replacing)
            {
                return true;
            }
        }

        return false;
    }
}