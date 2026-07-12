using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    // Easily expose child managers through properties
    public ChunkManager ChunkManager;
    public GameManager GameManager;
    public GameVariables GameVariables;
    public GameDatabase GameDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if(!ChunkManager || !GameManager || !GameDatabase || !GameVariables)
        {
            Debug.LogError("One or more singletons are not set!", this);
        }
    }
}
