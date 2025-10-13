using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    // Controllers
    public PauseController PauseController { get; private set; }
    public PlayerState playerState { get; private set; }

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
        playerState.plants = new PlantState[]
        {
            new PlantState { plantNameES = "Orquídea", plantNameEN = "Orchid" },
            new PlantState { plantNameES = "Uña de Gato", plantNameEN = "Cat's Claw" },
            new PlantState { plantNameES = "Cacao", plantNameEN = "Cocoa" }
        };
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

    public class PlayerState
    {
        public bool hasCollectedSeeds = false;
        public bool burnedPlot = false;
        public bool restoredPlot = false;
        public bool isAnyPlantBurning = false;

         public PlantState[] plants;
    }

    [System.Serializable]
    public class PlantState
    {
        public string plantNameES;
        public string plantNameEN;
        public bool isBurning = false;
        public bool isBurned = false;

        public string GetName(bool isSpanish)
        {
            return isSpanish ? plantNameES : plantNameEN;
        }
    }
}
