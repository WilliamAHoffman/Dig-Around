using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Scriptable Objects/Biome")]
public class Biome : ObjectID
{
    public override ObjectIDType Type => ObjectIDType.Biome;
    public List<Feature> wallFeatures;
    public List<Feature> floorFeatures;

    [Range(0f, 1f)]
    public float minNoiseValue;

    [Range(0f, 1f)]
    public float maxNoiseValue = 1f;

    public bool Matches(float noiseValue)
    {
        return noiseValue >= minNoiseValue && noiseValue <= maxNoiseValue;
    }

    private string GenerateTile(string replacing, List<Feature> features, Vector2Int location, int seed)
    {
        string newTile = replacing;

        if (features == null)
            return newTile;

        foreach (Feature feature in features)
        {
            if (feature == null)
                continue;

            newTile = feature.GenerateTile(newTile, location, seed);
        }

        return newTile;
    }

    public string GenerateFloorTile(string replacing, Vector2Int location, int seed)
    {
        return GenerateTile(replacing, floorFeatures, location, seed);
    }

    public string GenerateWallTile(string replacing, Vector2Int location, int seed)
    {
        return GenerateTile(replacing, wallFeatures, location, seed);
    }
}