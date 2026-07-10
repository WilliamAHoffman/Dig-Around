using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] bool randomSeed;

    //starting spawn area
    [SerializeField] private int startSizeX;
    [SerializeField] private int startSizeY;
    [SerializeField] private Vector2Int origin;
    [SerializeField] private bool renderStartChunks;

    //external references
    [SerializeField] private GameController gameController;
    public ChunkManager ChunkManager => gameController.ChunkManager;
    public GameVariables GameVariables => gameController.GameVariables;
    public WorldDataObjectDataBase WorldDataObjectDataBase => gameController.WorldDataObjectDataBase;
    public int ChunkSize => GameVariables.chunkSize; 
    public int WorldSeed => GameVariables.worldSeed; 

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

    public void CreateWorld()
    {
        SetUpGame();
        ChunkManager.DeleteAllChunks();
        ChunkManager.CreateBox(new Vector2Int(-startSizeX, -startSizeY) + origin, new Vector2Int(startSizeX, startSizeY) + origin, renderStartChunks);
    }

    public void SetUpGame()
    {

        if (randomSeed) GameVariables.SetNewWorldSeed();
        
        WorldDataObjectDataBase.Initialize();

        ChunkManager.DeleteAllChunks();

        foreach (NoiseSettings noise in WorldDataObjectDataBase.GetAllAssetsOfType<NoiseSettings>())
        {
            noise.CreateNoise();
        }
    }

    public void DeleteWorld()
    {
        ChunkManager.DeleteAllChunks();
    }
}