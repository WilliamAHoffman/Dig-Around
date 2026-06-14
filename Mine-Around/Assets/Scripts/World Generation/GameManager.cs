using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int worldSeed;
    [SerializeField] bool randomSeed;

    [SerializeField] int maxSeed;
    [SerializeField] int minSeed;

    public WorldGenerator worldGenerator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void CreateWorld()
    {
        ChunkManager.Instance.DeleteAllChunks();
        if (randomSeed)
        {
            worldSeed = Random.Range(minSeed, maxSeed);
        }

        Random.InitState(worldSeed);
        WorldDataRegistry.Instance.SetNoise();

        ChunkManager.Instance.CreateRadiusAt(new Vector2Int(0,0));
    }

    public int GetNewSeed()
    {
        return Random.Range(minSeed, maxSeed);
    }

    public int GetNumber(int min, int max)
    {
        return Random.Range(min, max);
    }
}