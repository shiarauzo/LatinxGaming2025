using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private GlobalUIManager UIManager;

    [Header("UI Panels")]
    [SerializeField] private GameObject creditsPanel;

    void Start()
    {
        UIManager = FindObjectOfType <GlobalUIManager>();
    }
    public void StartGame()
    {
        Debug.Log("StartGame pressed!");
        // Cargar GlobalUI
        if (FindObjectOfType<GlobalUIManager>() == null)
        {
            SceneManager.LoadSceneAsync("GlobalUI", LoadSceneMode.Additive);
        }

        // Carga IntroCutScene encima (sin eliminar GlobalUI).
        // Luego descarga MainMenu para liberar memoria.
        SceneManager.LoadScene("IntroCutScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void OpenSettings()
    {
        if (UIManager != null)
        {
            UIManager.ShowSettings();
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró GlobalUIManager en la escena.");
        }
    }


    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        // Resetear scroll automático
        creditsPanel.GetComponentInChildren<CreditScroll>().ResetScroll();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
