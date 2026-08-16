using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WorldGenerator",
    menuName = "World Generation/World Generator"
)]
public class WorldGenerator : ScriptableObject
{
    [Header("Layers")]
    [SerializeField]
    private List<GenerationLayer> worldLayers = new();

    [Header("World Sampling")]
    [SerializeField]
    private WorldSampler worldSampler;

    [Header("Database")]
    [SerializeField]
    private GameDatabase gameDatabase;

    public Chunk GenerateChunk(
        Vector2Int chunkPosition,
        Chunk chunk)
    {
        if (!ValidateReferences())
        {
            return chunk;
        }

        if (chunk == null)
        {
            Debug.LogError(
                "Cannot generate a null chunk.",
                this
            );

            return null;
        }

        TileDataAsset defaultTile =
            gameDatabase.GetDefaultAsset<TileDataAsset>();

        if (defaultTile == null)
        {
            Debug.LogError(
                "GameDatabase does not contain a default TileDataAsset.",
                this
            );

            return chunk;
        }

        int defaultTileID =
            defaultTile.ID;

        for (int y = 0; y < chunk.ChunkSize; y++)
        {
            for (int x = 0; x < chunk.ChunkSize; x++)
            {
                Vector2Int localPosition =
                    new(x, y);

                Vector2Int worldPosition =
                    ChunkUtilities.LocalToWorldCoord(
                        localPosition,
                        chunkPosition,
                        chunk.ChunkSize
                    );

                MapCell result =
                    GenerateLocation(
                        worldPosition,
                        defaultTileID,
                        defaultTileID
                    );

                chunk.SetCell(
                    localPosition,
                    result
                );
            }
        }

        return chunk;
    }

    #region Location Generation

    private MapCell GenerateLocation(
        Vector2Int worldPosition,
        int defaultFloor,
        int defaultWall)
    {
        WorldSample worldSample =
            worldSampler.Sample(
                worldPosition
            );

        MapCell result =
            new(
                defaultFloor,
                defaultWall
            );

        if (worldLayers == null)
        {
            return result;
        }

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

    #endregion

    #region Validation

    private bool ValidateReferences()
    {
        if (worldSampler == null)
        {
            Debug.LogError(
                $"{name} is missing a WorldSampler.",
                this
            );

            return false;
        }

        if (gameDatabase == null)
        {
            Debug.LogError(
                $"{name} is missing a GameDatabase.",
                this
            );

            return false;
        }

        return true;
    }

    #endregion
}