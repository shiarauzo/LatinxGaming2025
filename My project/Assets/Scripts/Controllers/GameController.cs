using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    // Controllers
    public PauseController PauseController { get; private set; }

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
