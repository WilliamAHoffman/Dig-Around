using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeGeneration", menuName = "Scriptable Objects/BiomeGeneration")]
public class BiomeGeneration : ScriptableObject
{
    public List<FeatureGenerator> features;
    /*
    public Chunk GenerateBiome(Chunk chunk)
    {
        foreach(FeatureGenerator feature in features)
        {
            feature.GenerateChunkFeatures()
        }
    }
    */
}
