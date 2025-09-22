using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    public void Play()
    {
         SceneManager.LoadScene("PrincipalMap");
    }

    public void Quit()
    {
        Debug.Log("Quit Game"); 
        Application.Quit();
    }

}
