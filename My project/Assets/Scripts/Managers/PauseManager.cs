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
            pausePanel.SetActive(false);
    }

    #region Panel + Pausa
    // Permitir mostrar u ocultar directamente el panel desde fuera
    public void SetPanelVisible(bool show)
    {
        if (show)
            ShowPanelAndPause();
        else
            HidePanelAndResume();
    }

    // Mostrar panel y pausar todo
    public void ShowPanelAndPause()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Lógica de pausa centralizada
        if (GameController.Instance == null)
        {
            Debug.LogError("GameController no existe todavía!");
        }
        else
        {
            GameController.Instance.PauseController.SetPause(true);
        }
    }

    // Ocultar panel y reanudar todo
    public void HidePanelAndResume()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (GameController.Instance == null)
        {
            Debug.LogError("GameController no existe todavía!");
        }
        else
        {
            GameController.Instance.PauseController.SetPause(false);
        }
    }
    #endregion

    #region Panel toggle
    public void TogglePausePanel()
    {
        if (pausePanel == null) return;

        bool isActive = pausePanel.activeSelf;
        SetPanelVisible(!isActive);
    }

    #endregion

    #region Botones UI
    public void OnResumePressed()
    {
        HidePanelAndResume();
        if (GlobalUIManager.Instance != null)
            GlobalUIManager.Instance.TogglePausePanel(false);
    }

    public void OnQuitPressed()
    {
        HidePanelAndResume();
        if (GlobalUIManager.Instance != null)
            GlobalUIManager.Instance.TogglePausePanel(false);
        ResetSingletons();

        // Cargar Boot en modo Single → destruye todas las demás escenas
        UnityEngine.SceneManagement.SceneManager.LoadScene("Boot", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    #endregion

    #region Helpers
    public static void ResetSingletons()
    {
        if (GlobalUIManager.Instance != null) Destroy(GlobalUIManager.Instance.gameObject);
        if (Instance != null) Destroy(Instance.gameObject);
        if (SettingsManager.Instance != null) Destroy(SettingsManager.Instance.gameObject);
        if (LocalizationManager.Instance != null) Destroy(LocalizationManager.Instance.gameObject);
    }
    #endregion
}
