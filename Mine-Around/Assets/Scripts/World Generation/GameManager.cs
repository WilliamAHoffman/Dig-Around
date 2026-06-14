using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int worldSeed;
    [SerializeField] bool randomSeed;

    public int maxSeed;
    public int minSeed;

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
        if (randomSeed)
        {
            worldSeed = Random.Range(minSeed, maxSeed);
        }
        WorldDataRegistry.Instance.SetNoise();
        
        ChunkManager.Instance.DeleteAllChunks();
        ChunkManager.Instance.CreateRadiusAt(new Vector2Int(0,0));
    }
}