using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("StartGame pressed!");
        SceneManager.LoadScene("IntroCutscene");
    }

    public void OpenSettings()
    {
       Debug.Log("Settings menu opened");
    }


    public void OpenCredits()
    {
       Debug.Log("Credits menu opened");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
