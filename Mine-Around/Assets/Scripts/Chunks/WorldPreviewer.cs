using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ChunkPreviewer : MonoBehaviour
{
    public RawImage uiPreviewDisplay;
    private Texture2D previewTexture;

    public RawImage uiPreviewDisplayWalls;
    private Texture2D previewTextureWalls;

    public RawImage uiPreviewDisplayFloors;
    private Texture2D previewTextureFloors;

    public void SaveTexture(Texture2D texture, string filename)
    {
        byte[] png = texture.EncodeToPNG();

        string folder = Path.Combine(Application.dataPath, "PreviewImages");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, filename + ".png");

        File.WriteAllBytes(path, png);

        Debug.Log($"Saved texture to: {path}");
    }

    /// <summary>
    /// Converts a List of Lists into a fast 1D array and renders it to a texture.
    /// </summary>
    /// <param name="blocks">The 2D block data structure. Assumes format: blocks[y][x]</param>
    public void PreviewBox(Vector2Int pos1, Vector2Int pos2, bool render)
    {
        Vector2Int start = Vector2Int.Min(pos1, pos2);
        Vector2Int end = Vector2Int.Max(pos1, pos2);

        int width = (end.x - start.x) * ChunkManager.Instance.ChunkSize;
        int height = (end.y - start.y) * ChunkManager.Instance.ChunkSize;

        if (previewTexture == null || previewTexture.width != width || previewTexture.height != height)
        {
            // The 'false' parameter explicitly disables Mipmaps to save memory and time
            previewTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            previewTexture.filterMode = FilterMode.Point; // Sharp pixelated grids
            previewTexture.wrapMode = TextureWrapMode.Clamp;
        }

        if (previewTextureWalls == null || previewTextureWalls.width != width || previewTextureWalls.height != height)
        {
            // The 'false' parameter explicitly disables Mipmaps to save memory and time
            previewTextureWalls = new Texture2D(width, height, TextureFormat.RGBA32, false);
            previewTextureWalls.filterMode = FilterMode.Point; // Sharp pixelated grids
            previewTextureWalls.wrapMode = TextureWrapMode.Clamp;
        }

        if (previewTextureFloors == null || previewTextureFloors.width != width || previewTextureFloors.height != height)
        {
            // The 'false' parameter explicitly disables Mipmaps to save memory and time
            previewTextureFloors = new Texture2D(width, height, TextureFormat.RGBA32, false);
            previewTextureFloors.filterMode = FilterMode.Point; // Sharp pixelated grids
            previewTextureFloors.wrapMode = TextureWrapMode.Clamp;
        }

        Color[] wallPixels = new Color[width * height];
        Color[] floorPixels = new Color[width * height];
        Color[] combinedPixels = new Color[width * height];

        ChunkManager.Instance.CreateBox(start, end, render);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int worldPos = new(start.x + x, start.y + y);

                TileData floor = ChunkManager.Instance.GetFloorDataAtLocation(worldPos);
                TileData wall = ChunkManager.Instance.GetWallDataAtLocation(worldPos);

                int index = y * width + x;

                floorPixels[index] = floor.mapColor;
                wallPixels[index] = wall.mapColor;
                combinedPixels[index] = wall.transparent
                    ? floor.mapColor
                    : wall.mapColor;
            }
        }

        previewTexture.SetPixels(combinedPixels);
        previewTexture.Apply();

        previewTextureWalls.SetPixels(wallPixels);
        previewTextureWalls.Apply();

        previewTextureFloors.SetPixels(floorPixels);
        previewTextureFloors.Apply();

        if (uiPreviewDisplay != null)
        {
            uiPreviewDisplay.texture = previewTexture;
        }

        if (uiPreviewDisplayWalls != null)
        {
            uiPreviewDisplayWalls.texture = previewTextureWalls;
        }

        if (uiPreviewDisplayFloors != null)
        {
            uiPreviewDisplayFloors.texture = previewTextureFloors;
        }

        SaveTexture(previewTexture, "combined");
        SaveTexture(previewTextureWalls, "walls");
        SaveTexture(previewTextureFloors, "floors");
    }
}
