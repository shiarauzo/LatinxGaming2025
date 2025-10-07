using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    public void StartGame()
    {
        Debug.Log("StartGame pressed!");
        SceneManager.LoadScene("IntroCutscene");
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
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed!");
        Application.Quit();
    }
    
}
