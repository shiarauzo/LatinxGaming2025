using System;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public bool IsPaused { get; private set; }

    // Evento que notificará a cualquier controlador de escena
    public event Action<bool> OnPauseChanged;

    // Cambiar el estado de pausa del juego
    public void SetPause(bool pause)
    {
        if (IsPaused == pause) return;

        IsPaused = pause;

        Time.timeScale = pause ? 0f : 1f;
        AudioListener.pause = pause;

        OnPauseChanged?.Invoke(pause);
    }

    // Alternar pausa (lógica, no el panel)
    public void TogglePause()
    {
        SetPause(!IsPaused);
    }
}