using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Scriptable Objects/Biome")]
public class Biome : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Biome;
    public List<Feature> wallFeatures;
    public List<Feature> floorFeatures;

    private string GenerateTile(string replacing, List<Feature> features, Vector2Int location)
    {
        string newTile = replacing;

        if (features == null)
            return newTile;

        foreach (Feature feature in features)
        {
            if (feature == null)
                continue;

            newTile = feature.GenerateTile(newTile, location);
        }

        return newTile;
    }

    public string GenerateFloorTile(string replacing, Vector2Int location)
    {
        return GenerateTile(replacing, floorFeatures, location);
    }

    public string GenerateWallTile(string replacing, Vector2Int location)
    {
        return GenerateTile(replacing, wallFeatures, location);
    }
}