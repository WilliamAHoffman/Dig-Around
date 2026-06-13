using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] List<Biome> biomes;
    [SerializeField] List<float> biomeRanges;
    private FastNoiseLite biomeNoise;
    [SerializeField] NoiseSettings noiseSettings;
    public void SetNoise(int newSeed)
    {
        biomeNoise = noiseSettings.GetNoise(newSeed);
    }

    public Chunk GenerateChunk(Vector2Int chunkLocation, int chunkSize)
    {
        Chunk chunk = new Chunk(chunkSize);
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = localPos + chunkLocation * chunkSize;

                string biomeID = ChooseBiome(worldPos.x, worldPos.y);
                Biome biome = WorldDataRegistry.Instance.GetBiomeData(biomeID);

                LocationTiles worldFloor = biome.GenerateFloorTile(WorldDataRegistry.Instance.GetAirTiles(), worldPos);
                LocationTiles worldWall = biome.GenerateWallTile(worldFloor, worldPos);

                WorldTile nextWallTile = WorldDataRegistry.Instance.GetWorldTile(worldWall.wall);
                WorldTile nextFloorTile = WorldDataRegistry.Instance.GetWorldTile(worldWall.floor);

                chunk.SetWorldWallTile(localPos, nextWallTile);
                chunk.SetWorldFloorTile(localPos, nextFloorTile);
            }
        }
        return chunk;
    }

    private string ChooseBiome(int x, int y)
    {
        if (biomeNoise == null)
        {
            Debug.LogError($"Feature noise has not been initialized for biome generation", this);
            return WorldDataRegistry.Instance.GetEmptyBiome().nameID;
        }

        float noiseSample = biomeNoise.GetNoise(x, y);

        int count = Mathf.Min(biomes.Count, biomeRanges.Count);

        for (int i = 0; i < count; i++)
        {
            if (biomes[i] == null)
                continue;

            if (noiseSample <= biomeRanges[i])
                return biomes[i].nameID;
        }

        return WorldDataRegistry.Instance.GetEmptyBiome().nameID;
    }
}