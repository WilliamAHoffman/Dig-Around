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
        int defaultTile = gameDatabase.GetDefaultAsset<TileDataAsset>().ID;

        for (int y = 0; y < chunk.ChunkSize; y++)
        {
            for (int x = 0; x < chunk.ChunkSize; x++)
            {
                Vector2Int localPosition = new(x, y);

                Vector2Int worldPosition =
                    chunkLocation * chunk.ChunkSize +
                    localPosition;

                MapCell result = GenerateLocation(
                    worldPosition,
                    defaultTile,
                    defaultTile
                );

                chunk.SetChunkCellValues(
                    localPosition,
                    result
                );
            }
        }

        return chunk;
    }

    private MapCell GenerateLocation(
        Vector2Int worldPosition,
        int defaultFloor,
        int defaultWall
    )
    {
        WorldSample worldSample = worldSampler.Sample(worldPosition);

        MapCell result = new MapCell(defaultFloor, defaultWall);

        foreach (GenerationLayer layer in worldLayers)
        {
            if (layer == null)
            {
                continue;
            }

            result = layer.Generate(
                worldPosition,
                worldSample,
                result
            );
        }

        return result;
    }
}