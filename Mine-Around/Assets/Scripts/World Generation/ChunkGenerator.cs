using UnityEngine;
/*
public class ChunkGenerator : MonoBehaviour
{
    private int chunkSize;
    private int seed;
    public NoiseSettings noiseSettings;
    private Noise noise;

    void Start()
    {
        chunkSize = ChunkManager.Instance.chunkSize;
        seed = ChunkManager.Instance.seed;
        noise = new Noise(seed);

    }
    public Chunk FillChunk(WorldCoord location)
    {
        Chunk chunk = new Chunk(chunkSize);
        for(int x = 0; x < chunkSize; x++)
        {
            for(int y = 0; y < chunkSize; y++)
            {
                int blockIndex = GetGeneratedAt(location);
                if(blockIndex == 0)
                {
                    chunk.SetWorldFloorTile(new Vector2Int(x,y), ChunkManager.Instance.GetIndexFloorTile(1));
                }
                chunk.SetWorldWallTile(new Vector2Int(x,y), ChunkManager.Instance.GetIndexWallTile(blockIndex));
            }
        }

        return chunk;
    }

    private int GetGeneratedAt(WorldCoord location)
    {
        float sample = noise.SampleNoise(location.blockLocation.x, location.blockLocation.y, noiseSettings);
        int blockIndex = 0;
        if(sample >= 0f)
        {
            blockIndex = 0;
        }
        else if(sample < 0f)
        {
            blockIndex = 1;
        }
        //Debug.Log(sample);
        return blockIndex;
    }
}

*/