using UnityEngine;

public class Level1AudioManager: MonoBehaviour
{
    public AudioSource backgroundMusic, aux2, aux3, aux4, aux5;
    public AudioSource victoryCue, loseCue;
    private bool aux2Playing, aux3Playing, aux4Playing, aux5Playing;
    void Start()
    {
        // AUX1 siempre loop
        backgroundMusic.loop = true;
        backgroundMusic.Play();

        // Inicializar loop y flags para los demás
        aux2.loop = aux3.loop = aux4.loop = aux5.loop = true;
        aux2.volume = aux3.volume = aux4.volume = aux5.volume = 0;
        aux2Playing = aux3Playing = aux4Playing = aux5Playing = false;
    }
    
    public void UpdateIntensity(int burningParcels)
    {
        // Reset volumes
        aux2.volume = 0;
        aux3.volume = 0;
        aux4.volume = 0;
        aux5.volume = 0;

        // AUX2
        if (burningParcels >= 1 && burningParcels <= 3)
        {
            if (!aux2Playing) { aux2.Play(); aux2Playing = true; }
            aux2.volume = 1;
        }
        else { aux2.volume = 0; aux2Playing = false; }

        // AUX3
        if (burningParcels >= 4 && burningParcels <= 5)
        {
            if (!aux3Playing) { aux3.Play(); aux3Playing = true; }
            aux3.volume = 1;
        }
        else { aux3.volume = 0; aux3Playing = false; }

        //AUX4
        if (burningParcels >= 6 && burningParcels <= 7)
        {
            if (!aux4Playing) { aux4.Play(); aux4Playing = true; }
            aux4.volume = 1;
        } else { aux4.volume = 0; aux4Playing = false; }

        //AUX5
        if (burningParcels >= 8)
        {
            if (!aux5Playing) { aux5.Play(); aux5Playing = true; }
            aux5.volume = 1;
        }
        else { aux5.volume = 0; aux5Playing = false; }
    }

    public void PlayVictory() {
        PauseAllAux();
        victoryCue.Play();
    }
    public void PlayLose()
    {
        PauseAllAux();
        loseCue.Play();
    }

    private void PauseAllAux()
    {
        backgroundMusic.Pause();
        aux2.Pause();
        aux3.Pause();
        aux4.Pause();
        aux5.Pause();
    }
}