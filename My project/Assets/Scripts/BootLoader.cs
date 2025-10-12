using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    private async void Start()
    {
        // 1. Cargar GlobalUI solo una
        if (!SceneManager.GetSceneByName("GlobalUI").isLoaded)
        {
            await SceneManager.LoadSceneAsync("GlobalUI", LoadSceneMode.Additive);
        }
        
        // 2. Pasar al MainMenu
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
