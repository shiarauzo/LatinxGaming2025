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
    public CanvasGroup fadeCanvas;
    private CutsceneController cutsceneController;
    private GameObject settingsPanel;
    private GameObject pausePanel;

    void Awake()
    {
        Debug.Log($"[GlobalUI] Awake ejecutado en escena {gameObject.scene.name} con hijos: {transform.childCount}");


        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[GlobalUI] Destruyendo duplicado de {gameObject.scene.name}");
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        string name = scene.name;
        // Debug.Log($"🔄 Escena cargada: {name}");

        cutsceneController = FindAnyObjectByType<CutsceneController>();
        EnsureEventSystem();

        // 🔄 Reasignar referencias si se perdieron al cambiar de escena
        if (pauseButton == null || settingsButton == null || skipButton == null)
        {
            Debug.Log("♻️ Reasignando referencias de botones...");
            pauseButton = GameObject.Find("PauseButton");
            settingsButton = GameObject.Find("SettingsButton");
            skipButton = GameObject.Find("SkipButton");
        }

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
        }
        else if (name == "GlobalPause")
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

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            StartCoroutine(FadeCanvasCoroutine(false, 0.5f));
        }
    }

    private void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
            return;

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
        Debug.Log($"[UI Visibility] Ejecutando UpdateUIVisibility para escena: {sceneName}");

        if (skipButton == null || settingsButton == null || pauseButton == null)
        {
            Debug.LogWarning("Algunos botones UI no han sido asignados");
            return;
        }

        Debug.Log("scene name" + sceneName);
        if (sceneName == "GlobalPause" || sceneName == "GlobalSettings")
            return;

        switch (sceneName)
        {
            case "IntroCutScene":
                skipButton?.SetActive(true);
                settingsButton?.SetActive(true);
                pauseButton?.SetActive(true);
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
        if (cutsceneController != null)
        {
            //  Debug.Log("⏹ Deteniendo Timeline desde GlobalUI...");
            cutsceneController.SkipIntro();
            //     cutsceneController.GoToNextScene();
        }
        else
        {
            SceneManager.LoadScene("PrincipalMap");
        }
    }

    public void ShowPause()
    {
        Debug.Log("🟨 ShowPause called.");
        Scene pauseScene = SceneManager.GetSceneByName("GlobalPause");

        if (!pauseScene.isLoaded)
        {
            Debug.Log("🧩 Cargando escena GlobalPause (modo Additive)...");
            SceneManager.LoadScene("GlobalPause", LoadSceneMode.Additive);
            StartCoroutine(WaitAndShowPause());
            return;
        }

        if (PauseManager.Instance != null)
        {
            Debug.Log("✅ PauseManager encontrado, mostrando panel...");
            //PauseManager.Instance.TogglePausePanel();
            PauseManager.Instance.SetPanelVisible(true);
            return;
        }


        Debug.LogWarning("⚠️ PauseManager no encontrado (posiblemente no se cargó la escena GlobalPause o el prefab no tiene el script).");
        TogglePausePanel(true);

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
        Debug.Log("✅ PauseManager cargado. Sincronizando panel de pausa.");
        PauseManager.Instance.SetPanelVisible(true);
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

    public void FadeScreen(bool fadeIn, float duration)
    {
        StartCoroutine(FadeCanvasCoroutine(fadeIn, duration));
    }

    private IEnumerator FadeCanvasCoroutine(bool fadeIn, float duration)
    {
        if (fadeCanvas == null)
        {
            Debug.LogWarning("fadeCanvas no asignado en GlobalUIManager");
            yield break;
        }

        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.blocksRaycasts = true; // Evita clics durante el fade

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        fadeCanvas.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        fadeCanvas.alpha = endAlpha;

        // Si terminó en 0, puedes desactivarlo
        if (endAlpha == 0f)
        {
            fadeCanvas.gameObject.SetActive(false);
            fadeCanvas.blocksRaycasts = false;
        }
    }
}
