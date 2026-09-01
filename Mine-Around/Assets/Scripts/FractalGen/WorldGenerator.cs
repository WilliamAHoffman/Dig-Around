using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WorldGenerator",
    menuName = "World Generation/World Generator"
)]
public class WorldGenerator : ScriptableObject
{
    [Header("Layers")]
    [SerializeField]
    private List<FactalWorldLayer> worldLayers = new();

    [Header("Database")]
    [SerializeField] private GameDatabase<BloxelBase> bloxelDataBase ;

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

        BloxelBase defaultTile =
            bloxelDataBase.DefaultAsset;

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

        MapCell result =
            new(
                defaultFloor,
                defaultWall
            );

        if (worldLayers == null)
        {
            return result;
        }

        foreach (FactalWorldLayer layer in worldLayers)
        {
            if (layer == null)
            {
                continue;
            }

            result = layer.Generate(
                worldPosition,
                result
            );
        }

        return result;
    }

    #endregion

    #region Validation

    private bool ValidateReferences()
    {

        if (bloxelDataBase == null)
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