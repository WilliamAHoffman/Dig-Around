using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "FeatureGenerator", menuName = "Scriptable Objects/FeatureGenerator")]
public class FeatureGenerator : ScriptableObject
{
    public List<GenerationRule> floorRules;
    public List<GenerationRule> wallRules;
    public NoiseSettings noiseSettings;
    public Chunk GenerateChunkFeatures(int seed, Vector2Int location, Chunk chunk)
    {
        for(int x = 0; x < chunk.chunkSize; x++)
        {
            for(int y = 0; y < chunk.chunkSize; y++)
            {
                Vector2Int blockLocation = new Vector2Int(x + location.x, y + location.y);
                Vector2Int localPos = new Vector2Int(x, y);
                float noiseSample = Noise.SampleNoise(x,y,noiseSettings,seed);
                chunk.SetWorldWallTile(new Vector2Int(x,y), WorldManager.Instance.GetWorldWall(FindNextTile(chunk.GetWorldWallTile(localPos).nameID, wallRules, noiseSample)));

                chunk.SetWorldFloorTile(new Vector2Int(x,y), WorldManager.Instance.GetWorldFloor(FindNextTile(chunk.GetWorldFloorTile(localPos).nameID, floorRules, noiseSample)));
            }
        }
        return chunk;
    }

    private string FindNextTile(string replacing, List<GenerationRule> rules, float noiseSample)
    {
        foreach(GenerationRule rule in rules)
        {
            if(rule.Matches(noiseSample) && rule.Replaces(replacing))
            {
                return rule.tileID;
            }
        }
        return replacing;
    }
}

