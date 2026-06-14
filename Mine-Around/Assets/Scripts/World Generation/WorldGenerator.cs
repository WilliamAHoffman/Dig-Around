using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] List<Biome> biomes;
    [SerializeField] List<float> biomeRanges;
    [SerializeField] NoiseSettings biomeNoise;

    public Chunk GenerateChunk(Vector2Int chunkLocation, int chunkSize)
    {
        Chunk chunk = new Chunk(chunkSize);
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = localPos + chunkLocation * chunkSize;

                Biome biome = ChooseBiome(worldPos.x, worldPos.y);

                LocationTiles tiles = biome.GenerateTiles(WorldDataRegistry.Instance.GetAirTiles(), worldPos);

                WorldTile wallTile = WorldDataRegistry.Instance.GetWorldTile(tiles.wall);
                WorldTile floorTile = WorldDataRegistry.Instance.GetWorldTile(tiles.floor);

                chunk.SetWorldWallTile(localPos, wallTile);
                chunk.SetWorldFloorTile(localPos, floorTile);

            }
        }
        return chunk;
    }

    private Biome ChooseBiome(int x, int y)
    {
        if (biomeNoise == null)
        {
            Debug.LogError("Biome noise has not been initialized for biome generation.", this);
            return WorldDataRegistry.Instance.GetEmptyBiome();
        }

        float noiseSample = biomeNoise.Sample(x, y);

        if (biomes == null || biomeRanges == null)
            return WorldDataRegistry.Instance.GetEmptyBiome();

        int count = Mathf.Min(biomes.Count, biomeRanges.Count);

        for (int i = 0; i < count; i++)
        {
            Biome biome = biomes[i];

            if (biome == null)
                continue;

            if (noiseSample <= biomeRanges[i])
                return biome;
        }

        return WorldDataRegistry.Instance.GetEmptyBiome();
    }
}