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

    public void TogglePause()
    {
        if (pausePanel != null)
        {
            bool isActive = pausePanel.activeSelf;
            pausePanel.SetActive(!isActive);

            if (!isActive)
            {
                Time.timeScale = 0f;
                AudioListener.pause = true;
            }
            else
            {
                Time.timeScale = 1f;
                AudioListener.pause = false;
            }
        }
    }

    public void OnResumePressed()
    {
        GlobalUIManager.Instance.TogglePausePanel(false);
    }

    public void OnQuitPressed()
    {
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
