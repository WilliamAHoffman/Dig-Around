using System.Collections.Generic;
using TreeEditor;
using Unity.Collections;
using UnityEngine;
/*
public class WorldGenerator : MonoBehaviour
{
    public int seed;
    public bool randomSeed;
    int chunkSize;
    List<string> biomes;
    NoiseSettings biomeNoise;
    void Start()
    {
        if (randomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }
    }
    public Chunk GenerateChunk(Vector2Int chunkLocation)
    {
        Chunk chunk = new Chunk(chunkSize);
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {

                Vector2Int localPos = new Vector2Int(x, y);
                Biome biome = ChooseBiome(localPos + chunkLocation * chunkSize);
                WorldWallTile worldWall = biome.GenerateWallTile("",);
                WorldFloorTile worldFloor = 

                RuleTile nextWallTile = WorldManager.Instance.GetWallData(worldWall.nameID).tile;
                RuleTile nextFloorTile = WorldManager.Instance.GetWallData(worldFloor.nameID).tile;
                i++;
            }
        }
    }

    private string ChooseBiome(Vector2Int location)
    {
        float noiseSample = Noise.SampleNoise(location.x,location.y,biomeNoise,seed);
        foreach(string biome in biomes)
        {
            if(biome.Matches(noiseSample))
            {
                return biome.biomeID;
            }
        }
        return "";
    }
}
*/