using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenerator", menuName = "Scriptable Objects/WorldGenerator")]
public class WorldGenerator : ScriptableObject
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

                LocationTiles tiles = biome.GenerateTiles(WorldDataObjectDataBase.Instance.GetDefaultAsset<TileData>().LocationTiles(), worldPos);

                chunk.SetWorldLocationTile(localPos, tiles);

            }
        }
        return chunk;
    }

    private Biome ChooseBiome(int x, int y)
    {
        if (biomeNoise == null)
        {
            Debug.LogError("Biome noise has not been initialized for biome generation.", this);
            return WorldDataObjectDataBase.Instance.GetDefaultAsset<Biome>();
        }

        float noiseSample = biomeNoise.Sample(x, y);

        if (biomes == null || biomeRanges == null)
            return WorldDataObjectDataBase.Instance.GetDefaultAsset<Biome>();

        int count = Mathf.Min(biomes.Count, biomeRanges.Count);

        for (int i = 0; i < count; i++)
        {
            Biome biome = biomes[i];

            if (biome == null)
                continue;

            if (noiseSample <= biomeRanges[i])
                return biome;
        }

        return WorldDataObjectDataBase.Instance.GetDefaultAsset<Biome>();
    }
}