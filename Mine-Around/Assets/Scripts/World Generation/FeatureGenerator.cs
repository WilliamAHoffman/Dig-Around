using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FeatureGenerator : MonoBehaviour
{
    public NoiseSettings noiseSettings;
    public List<WallGenerationRule> wallRules;
    public List<FloorGenerationRule> floorRules;
    private Noise noise;
    public Chunk GenerateChunkFeatures(int chunkSize, int seed)
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
}

