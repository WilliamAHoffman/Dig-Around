using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatureGenerator", menuName = "Scriptable Objects/FeatureGenerator")]
public class FeatureGenerator : ScriptableObject
{
    public List<GenerationRule> floorRules;
    public List<GenerationRule> WallRules;
    private Noise noise;
    public Chunk GenerateChunkFeatures(int chunkSize, int seed, Vector2Int location)
    {
        if(noise == null)
        {
            noise = new Noise(seed);
        }
        Chunk chunk = new Chunk(chunkSize);
        for(int x = 0; x < chunkSize; x++)
        {
            for(int y = 0; y < chunkSize; y++)
            {
                chunk.SetWorldFloorTile(new Vector2Int(x,y), WorldManager.Instance.GetWorldFloor("air"));
                chunk.SetWorldWallTile(new Vector2Int(x,y), WorldManager.Instance.GetWorldWall("air"));
            }
        }

        return chunk;
    }
    /*
    private FindNextTile(Vector2Int blockLocation)
    {
        float noiseSample = noise.SampleNoise(blockLocation.x, blockLocation.y);
        foreach(WallGenerationRule rule in wallRules)
        {
            if()
        }
    }
    */
}

