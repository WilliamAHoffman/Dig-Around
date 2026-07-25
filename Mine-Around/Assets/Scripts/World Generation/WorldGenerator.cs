using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenerator", menuName = "World Generation/World Generator")]
public class WorldGenerator : ScriptableObject
{
    [Header("Layers")]
    [SerializeField] private List<GenerationLayer> worldLayers;
    [Header("Noise Sample")]
    [SerializeField] private WorldSampler worldSampler;
    public GameDatabase gameDatabase;

    public Chunk GenerateChunk(Vector2Int chunkLocation, Chunk chunk)
    {
        for (int x = 0; x < chunk.chunkSize; x++)
        {
            for (int y = 0; y < chunk.chunkSize; y++)
            {
                Vector2Int localPos = new Vector2Int(x, y);
                Vector2Int worldPos = localPos + chunkLocation * chunk.chunkSize;
                GenerationResult result = GenerateLocation(worldPos);

                chunk.SetChunkCellValues(localPos, result.MapCellValues());
            }
        }

        return chunk;
    }

    private GenerationResult GenerateLocation(Vector2Int worldPos)
    {

        if (worldSampler == null)
        {
            Debug.LogError("WorldGenerator has no WorldSampler assigned.", this);
            return default;
        }

        WorldSample worldSample = worldSampler.Sample(worldPos);
        GenerationResult result = new GenerationResult(gameDatabase.GetDefaultAsset<TileDataAsset>(), gameDatabase.GetDefaultAsset<TileDataAsset>());

        if (worldLayers == null)
            return result;

        foreach (GenerationLayer layer in worldLayers)
        {
            result = layer.Generate(worldPos, worldSample, result);
        }
        return result;
    }
}