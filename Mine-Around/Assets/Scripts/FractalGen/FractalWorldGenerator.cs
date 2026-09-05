using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FractalWorldGenerator",
    menuName = "World Generation/Fractal World Generator")]
public class FractalWorldGenerator : ScriptableObject
{
    [Header("Generation")]
    [SerializeField] private GameVariables gameVariables;
    private int Seed => gameVariables.worldSeed;

    [Header("Layers")]
    [SerializeField] private List<FactalWorldLayer> worldLayers = new();

    [Header("Database")]
    [SerializeField] private GameDatabase<BloxelBase> bloxelDataBase;

    public Chunk GenerateChunk(Vector2Int chunkPosition, Chunk chunk)
    {
        if (!ValidateReferences())
            return chunk;

        if (chunk == null)
        {
            Debug.LogError("Cannot generate a null chunk.", this);
            return null;
        }

        BloxelBase defaultTile = bloxelDataBase.DefaultAsset;

        if (defaultTile == null)
        {
            Debug.LogError(
                "GameDatabase does not contain a default BloxelBase asset.",
                this);
            return chunk;
        }

        int defaultTileID = defaultTile.ID;

        for (int y = 0; y < chunk.ChunkSize; y++)
        {
            for (int x = 0; x < chunk.ChunkSize; x++)
            {
                Vector2Int localPosition = new(x, y);
                Vector2Int worldPosition = ChunkUtilities.LocalToWorldCoord(
                    localPosition,
                    chunkPosition,
                    chunk.ChunkSize);

                MapCell result = GenerateLocation(worldPosition, defaultTileID);
                chunk.SetCell(localPosition, result);
            }
        }

        return chunk;
    }

    private MapCell GenerateLocation(Vector2Int worldPosition, int defaultTileID)
    {
        MapCell cell = new(defaultTileID, defaultTileID);
        GenerationContext context = new(worldPosition, cell, Seed);

        if (worldLayers == null)
            return context.Cell;

        foreach (FactalWorldLayer layer in worldLayers)
        {
            if (layer == null)
                continue;

            layer.Generate(ref context);
        }

        return context.Cell;
    }

    private bool ValidateReferences()
    {
        if (bloxelDataBase == null)
        {
            Debug.LogError($"{name} is missing a GameDatabase.", this);
            return false;
        }

        return true;
    }
}
