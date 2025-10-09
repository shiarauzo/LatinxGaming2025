using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

   // SceneManager.LoadSceneAsync("GlobalUI", LoadSceneMode.Additive);

    public void StartGame()
    {
        Debug.Log("StartGame pressed!");
        // Cargar GlobalUI
        if (FindObjectOfType<GlobalUIManager>() == null)
        {
            Debug.Log("🔄 Cargando GlobalUI...");
            SceneManager.LoadSceneAsync("GlobalUI", LoadSceneMode.Additive);
        } else
        {
            Debug.Log("✅ GlobalUI ya cargado.");
        }

        SceneManager.LoadScene("IntroCutScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings menu opened");
        optionsPanel.SetActive(true);
    }


    public void OpenCredits()
    {
        Debug.Log("Credits menu opened");
        creditsPanel.SetActive(true);
        // Resetear scroll automático
        creditsPanel.GetComponentInChildren<CreditScroll>().ResetScroll();
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed!");
        Application.Quit();
    }
    
}
