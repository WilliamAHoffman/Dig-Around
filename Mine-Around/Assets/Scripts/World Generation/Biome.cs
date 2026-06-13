using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Scriptable Objects/Biome")]
public class Biome : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Biome;
    public List<Feature> wallFeatures;
    public List<Feature> floorFeatures;

    public LocationTiles GenerateFloorTile(LocationTiles currentTiles, Vector2Int location)
    {

        if (floorFeatures == null)
            return currentTiles;

        LocationTiles newTiles = currentTiles;

        foreach (Feature feature in floorFeatures)
        {
            if (feature == null)
                continue;
            
            newTiles = feature.GenerateTiles(newTiles, location);
        }

        return newTiles;
    }

    public LocationTiles GenerateWallTile(LocationTiles currentTiles, Vector2Int location)
    {
        if (wallFeatures == null)
            return currentTiles;

        LocationTiles newTiles = currentTiles;

        foreach (Feature feature in wallFeatures)
        {
            if (feature == null)
                continue;
            
            newTiles = feature.GenerateTiles(newTiles, location);
        }

        return newTiles;
    }
}