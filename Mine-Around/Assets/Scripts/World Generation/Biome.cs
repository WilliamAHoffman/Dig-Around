using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Scriptable Objects/Biome")]
public class Biome : WorldDataObject
{
    public List<Feature> features;

    public LocationTiles GenerateTiles(LocationTiles currentTiles, Vector2Int location)
    {

        if (features == null)
            return currentTiles;

        LocationTiles newTiles = currentTiles;

        foreach (Feature feature in features)
        {
            if (feature == null)
                continue;
            
            newTiles = feature.GenerateTiles(newTiles, location);
        }

        return newTiles;
    }
}