// Parte Visual
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    [SerializeField] private GameObject pausePanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    // Enlace con PauseController
    public void SetPause(bool pause)
    {
        PauseController.Instance?.SetPause(pause);
    }

    // Alternar pausa (lógica, no el panel)
    public void TogglePauseState()
    {
       PauseController.Instance?.TogglePause();
    }

    // Panel visual
    public void TogglePausePanel()
    {
        if (pausePanel == null) return;

        bool isActive = pausePanel.activeSelf;
        pausePanel.SetActive(!isActive);

        SetPause(!isActive);
    }

    // Permitir mostrar u ocultar directamente el panel desde fuera (sin toggle)
    public void SetPanelVisible(bool show)
    {
        if (pausePanel == null) return;
        pausePanel.SetActive(show);
        SetPause(show); // sincroniza TimeScale y Audio
    }

    // Pausa completa: pausa el juego y muestra el panel
    public void FullPause()
    {
        SetPause(true);
        TogglePausePanel();
    }

    // Reanuda completa: reanuda el juego y oculta el panel
    public void FullResume()
    {
       SetPause(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public static bool IsGamePaused()
    {
        return PauseController.Instance != null && PauseController.Instance.IsPaused;
    }

    public void OnResumePressed()
    {
        FullResume();
        GlobalUIManager.Instance.TogglePausePanel(false);
    }

    public void OnQuitPressed()
    {
        FullResume();
        GlobalUIManager.Instance.TogglePausePanel(false);
        ResetSingletons();
        
        // Cargar Boot en modo Single → destruye todas las demás escenas
        UnityEngine.SceneManagement.SceneManager.LoadScene("Boot", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public static void ResetSingletons()
    {
        if(GlobalUIManager.Instance != null) Destroy(GlobalUIManager.Instance.gameObject);
        if(Instance != null) Destroy(Instance.gameObject);
        if(SettingsManager.Instance != null) Destroy(SettingsManager.Instance.gameObject);
        if(LocalizationManager.Instance != null) Destroy(LocalizationManager.Instance.gameObject);
    }
}
