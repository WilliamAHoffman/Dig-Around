using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int seed;
    public bool randomSeed;
    void Start()
    {
        if (randomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }
    }
    public Chunk GenerateChunk(Vector2Int chunkLocation, int chunkSize)
    {
        Chunk chunk = new Chunk(chunkSize);
        return chunk;
    }
}
