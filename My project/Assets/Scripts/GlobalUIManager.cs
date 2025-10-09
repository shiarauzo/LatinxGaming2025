using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Xml.Serialization;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

// Controlar la visibilidad de los botones
public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager Instance;
    [Header("UI Elements")]
    public GameObject pauseButton;
    public GameObject settingsButton;
    public GameObject skipButton;
    private CutsceneController cutsceneController;

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        EnsureEventSystem();

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("🚀 GlobalUIManager Awake ejecutándose...");
    }

    private void Start()
    {
        UpdateUIVisibility(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;
        Debug.Log($"🔄 Escena cargada: {name}");

        cutsceneController = FindAnyObjectByType<CutsceneController>();
        // 🔹 Asegurar que haya EventSystem
        EnsureEventSystem();
        UpdateUIVisibility(name);
    }

    private void EnsureEventSystem()
    {
        EventSystem existingES = FindAnyObjectByType<EventSystem>();

        if (existingES != null)
        {
            Debug.Log("⚙️ EventSystem ya existe en escena.");
            return;
        }

        // Crear nuevo EventSystem compatible con el nuevo Input System
        GameObject es = new GameObject("EventSystem", typeof(EventSystem));
        es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        DontDestroyOnLoad(es);
        Debug.Log("✅ EventSystem creado por GlobalUIManager (nuevo Input System)");



    }
    
    private void UpdateUIVisibility(string sceneName)
    {
        if (skipButton == null || settingsButton == null || pauseButton == null)
        {
            Debug.LogWarning("Algunos botones UI no han sido asignados");
            return;
        }

        switch (sceneName)
        {
            case "IntroCutScene":
                skipButton?.SetActive(true);
                settingsButton?.SetActive(true);
                pauseButton?.SetActive(false);
                break;

            case "PrincipalMap":
                skipButton?.SetActive(false);
                pauseButton?.SetActive(true);
                settingsButton?.SetActive(true);
                break;
            case "MainMenu":
            default:
                skipButton.SetActive(false);
                settingsButton.SetActive(false);
                pauseButton?.SetActive(false);
                break;
        }
    }

    public void SkipIntro()
    {
        Debug.Log("Intro skipped by player.");

        if (cutsceneController != null)
        {
            Debug.Log("⏹ Deteniendo Timeline desde GlobalUI...");
            cutsceneController.StopIntro();
        }

        SceneManager.LoadScene("PrincipalMap");
    }

    public void PauseGame()
    {
        Debug.Log("PAUSE WAS PRESSED.");
    }
    
    public void ShowSettings()
    {
         Debug.Log("SHOW SETTINGS WAS PRESSED.");
    }
}
