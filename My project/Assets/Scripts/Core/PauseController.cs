using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Cambiar el estado de pausa del juego
    public void SetPause(bool pause)
    {
        if (IsPaused == pause) return;

        IsPaused = pause;

        Time.timeScale = pause ? 0f : 1f;
        AudioListener.pause = pause;
    }

    // Alternar pausa (lógica, no el panel)
    public void TogglePause()
    {
        SetPause(!IsPaused);
    }
}