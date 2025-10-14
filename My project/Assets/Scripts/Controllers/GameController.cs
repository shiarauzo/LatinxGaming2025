using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    // Controllers
    public PauseController PauseController { get; private set; }
    public PlayerState playerState { get; private set; }

    public bool hasWon = false;
    public bool hasLost = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    
    private static void Initialize()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("GameController");
            Instance = go.AddComponent<GameController>();
            DontDestroyOnLoad(go);
            Instance.InitializeControllers();
        }
    }

    private void InitializeControllers()
    {
        // Crear PauseController si no existe
        if (PauseController == null)
        {
            GameObject pauseGo = new GameObject("PauseController");
            PauseController = pauseGo.AddComponent<PauseController>();
            DontDestroyOnLoad(pauseGo);
        }

        playerState = new PlayerState();
        // Inicializar 3 especies, cada una con 9 parcelas
        playerState.plantSpecies = new PlantState[3][];

        string[] speciesNamesES = { "Orquídea", "Uña de Gato", "Cacao" };
        string[] speciesNamesEN = { "Orchid", "Cat's Claw", "Cocoa" };

        for (int i = 0; i < 3; i++)
    {
        playerState.plantSpecies[i] = new PlantState[9]; // 9 parcelas por especie
        for (int j = 0; j < 9; j++)
        {
            playerState.plantSpecies[i][j] = new PlantState
            {
                plantNameES = speciesNamesES[i],
                plantNameEN = speciesNamesEN[i],
                isBurning = false,
                isBurned = false,
                isRestored = false
            };
        }
    }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeControllers();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        Debug.Log("GameController listo");
    }
}
