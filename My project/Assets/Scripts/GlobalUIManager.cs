using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;


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
    private GameObject settingsPanel;
    private GameObject pausePanel;

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
        EnsureEventSystem();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Cargar la escena 'GlobalSettings'
        if (!SceneManager.GetSceneByName("GlobalSettings").isLoaded)
            SceneManager.LoadScene("GlobalSettings", LoadSceneMode.Additive);
        
        UpdateUIVisibility(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;
       // Debug.Log($"🔄 Escena cargada: {name}");

        cutsceneController = FindAnyObjectByType<CutsceneController>();
        EnsureEventSystem();
        UpdateUIVisibility(name);

        // Si la escena cargada es GlobalSettings, encuentra el panel de settings
        if (name == "GlobalSettings")
        {
            settingsPanel = FindPanelInScene(scene, "GlobalSettingsRoot", "SettingsCanvas/SettingsPanel");
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
                Debug.Log("✅ SettingsPanel encontrado y oculto por defecto.");
            }
        } else if (name == "GlobalPause")
        {
            if (pausePanel == null)
            {
                pausePanel = FindPanelInScene(scene, "GlobalPauseRoot", "PauseCanvas/PausePanel");
                if (pausePanel != null)
                {
                    pausePanel.SetActive(false);
                    Debug.Log("✅ PausePanel encontrado y oculto por defecto.");
                }
            }
        }
    }

    private void EnsureEventSystem()
    {
        EventSystem existingES = FindAnyObjectByType<EventSystem>();

        if (existingES != null)
        {
            Debug.Log("EventSystem ya existe en escena.");
            return;
        }

        // Crear nuevo EventSystem compatible con el nuevo Input System
        GameObject es = new GameObject("EventSystem", typeof(EventSystem));
        es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        DontDestroyOnLoad(es);
    }
    
    private void UpdateUIVisibility(string sceneName)
    {
        if (skipButton == null || settingsButton == null || pauseButton == null)
        {
            Debug.LogWarning("Algunos botones UI no han sido asignados");
            return;
        }

        Debug.Log("scene name" + sceneName);
        if(sceneName == "GlobalPause" || sceneName == "GlobalSettings")
            return;

        switch (sceneName)
        {
            case "IntroCutScene":
                skipButton?.SetActive(true);
                settingsButton?.SetActive(true);
                pauseButton?.SetActive(true); //update
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

    public void ShowPause()
    {
        Debug.Log("PAUSE WAS PRESSED.");
        Scene pauseScene = SceneManager.GetSceneByName("GlobalPause");

        if (!pauseScene.isLoaded)
        {
            SceneManager.LoadScene("GlobalPause", LoadSceneMode.Additive);
            StartCoroutine(WaitAndShowPause());
            return;
        }

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.TogglePause();
        }
        else
        {
            Debug.LogWarning("PauseManager no encontrado.");
        }
    }

    public void TogglePausePanel(bool show)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(show);
            Time.timeScale = show ? 0f : 1f;
            AudioListener.pause = show;
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró PausePanel en GlobalPause.");
        }
    }
    public void ShowSettings()
    {
        Scene settingsScene = SceneManager.GetSceneByName("GlobalSettings");

        if (!settingsScene.isLoaded)
        {
            // Debug.Log("🧩 Cargando escena GlobalSettings (modo Additive)...");
            SceneManager.LoadScene("GlobalSettings", LoadSceneMode.Additive);
            return;
        }

        ToggleSettingsPanel(true);
    }

    public void ToggleSettingsPanel(bool show)
    {
        if (settingsPanel == null)
        {
            Scene settingsScene = SceneManager.GetSceneByName("GlobalSettings");
            if (settingsScene.isLoaded)
            {
                settingsPanel = FindPanelInScene(settingsScene, "GlobalSettingsRoot", "SettingsCanvas/SettingsPanel");
            }
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(show);
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró SettingsPanel en GlobalSettings.");
        }
    }

    private IEnumerator WaitAndShowPause()
    {
        yield return new WaitUntil(() => PauseManager.Instance != null);
        PauseManager.Instance.TogglePause();
    }

    // Buscar por jerarquía completa dentro de la escena cargada
    private GameObject FindPanelInScene(Scene scene, string rootName, string panelPath)
    {
        var rootObjects = scene.GetRootGameObjects();
        foreach (var root in rootObjects)
        {
            if (root.name == rootName)
            {
                var foundPanel = root.transform.Find(panelPath);
                if (foundPanel != null)
                {
                   return foundPanel.gameObject;
                }
            }
        }
        return null;
    }
}
