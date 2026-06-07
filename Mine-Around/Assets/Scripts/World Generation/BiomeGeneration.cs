using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeGeneration", menuName = "Scriptable Objects/BiomeGeneration")]
public class BiomeGeneration : ScriptableObject
{
    public List<FeatureGenerator> features;

    public Chunk GenerateBiome(Chunk chunk, Vector2Int chunkLocation, int seed)
    {
        foreach(FeatureGenerator feature in features)
        {
            chunk = feature.GenerateChunkFeatures(seed, chunkLocation, chunk);
        }

        return chunk;
    }
}
