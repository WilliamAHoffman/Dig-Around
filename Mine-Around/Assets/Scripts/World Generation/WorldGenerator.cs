using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenerator", menuName = "World Generation/World Generator")]
public class WorldGenerator : ScriptableObject
{
    [Header("Layers")]
    [SerializeField] private List<GenerationLayer> worldLayers;
    [Header("Noise Sample")]
    [SerializeField] private WorldSampler worldSampler;

    public Chunk GenerateChunk(Vector2Int chunkLocation, int chunkSize)
    {
        Chunk chunk = new Chunk(chunkSize);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = localPos + chunkLocation * chunkSize;
                GenerationResult result = GenerateLocation(worldPos);

                chunk.SetWorldLocationTile(localPos, result.LocationTiles());
            }
        }

        return chunk;
    }

    private GenerationResult GenerateLocation(Vector2Int worldPos)
    {
        WorldSample worldSample = worldSampler.Sample(worldPos);
        GenerationResult result = new GenerationResult(WorldDataObjectDataBase.Instance.GetDefaultAsset<TileData>(),WorldDataObjectDataBase.Instance.GetDefaultAsset<TileData>());
        foreach(GenerationLayer layer in worldLayers)
        {
            if(layer.Generates(worldSample))
            {
                result = layer.Generate(worldPos, worldSample, result);
            }
        }
        return result;
    }
}