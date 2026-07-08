using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BasicSeed worldSeed;
    [SerializeField] bool randomSeed;
    [SerializeField] private int maxSeed;
    [SerializeField] private int minSeed;
    [SerializeField] private int startSizeX;
    [SerializeField] private int startSizeY;
    [SerializeField] private Vector2Int origin;
    [SerializeField] private bool renderStartChunks;

    public WorldGenerator worldGenerator;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetUpGame();
    }

    private int GetNewSeed()
    {
        return Random.Range(minSeed, maxSeed);
    }

    public void CreateWorld()
    {
        SetUpGame();
        ChunkManager.Instance.DeleteAllChunks();
        ChunkManager.Instance.CreateBox(new Vector2Int(-startSizeX, -startSizeY) + origin, new Vector2Int(startSizeX, startSizeY) + origin, renderStartChunks);
    }

    public void SetUpGame()
    {

        if (randomSeed) worldSeed.seed = GetNewSeed();
        
        WorldDataObjectDataBase.Instance.Initialize();

        ChunkManager.Instance.DeleteAllChunks();

        foreach (NoiseSettings noise in WorldDataObjectDataBase.Instance.GetAllAssetsOfType<NoiseSettings>())
        {
            noise.CreateNoise();
        }
    }

    public void DeleteWorld()
    {
        ChunkManager.Instance.DeleteAllChunks();
    }
}