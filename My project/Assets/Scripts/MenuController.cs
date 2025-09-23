using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LoreScene");
    }

    public void OpenSettings()
    {
       Debug.Log("Settings menu opened");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
