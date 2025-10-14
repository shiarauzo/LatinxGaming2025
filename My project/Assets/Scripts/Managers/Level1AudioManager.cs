using System.Collections;
using UnityEngine;

public class Level1AudioManager: MonoBehaviour
{
    public AudioSource backgroundMusic, aux2, aux3, aux4, aux5;
    public AudioSource victoryCue, loseCue;
    private Coroutine aux2Fade, aux3Fade, aux4Fade, aux5Fade;

    void Start()
    {
        // AUX1 siempre loop
        backgroundMusic.loop = true;
        backgroundMusic.Play();

        // Inicializar loop y flags para los demás
        aux2.loop = aux3.loop = aux4.loop = aux5.loop = true;
        aux2.volume = aux3.volume = aux4.volume = aux5.volume = 0;
    }

    public void UpdateIntensity(int burningParcels)
    {
        Debug.Log($"⚠️ Parcelas en PELIGRO: {burningParcels}");
        FadeAudio(aux2, burningParcels >= 1 ? 1f : 0f, ref aux2Fade);
        FadeAudio(aux3, burningParcels >= 4 ? 1f : 0f, ref aux3Fade);
        FadeAudio(aux4, burningParcels >= 6 ? 1f : 0f, ref aux4Fade);
        FadeAudio(aux5, burningParcels >= 8 ? 1f : 0f, ref aux5Fade);
    }
    
    private void FadeAudio(AudioSource source, float targetVolume, ref Coroutine fadeCoroutine)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToVolume(source, targetVolume, 1f));
    }
    private IEnumerator FadeToVolume(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        source.volume = targetVolume;
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

    public void FadeOutAuxTracksExceptBackground(float fadeDuration = 1.5f)
    {
        StartCoroutine(FadeOutRoutine(fadeDuration));
    }
    
    private IEnumerator FadeOutRoutine(float duration)
    {
        float startVol2 = aux2.volume;
        float startVol3 = aux3.volume;
        float startVol4 = aux4.volume;
        float startVol5 = aux5.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = 1f - (time / duration);

            aux2.volume = startVol2 * t;
            aux3.volume = startVol3 * t;
            aux4.volume = startVol4 * t;
            aux5.volume = startVol5 * t;

            yield return null;
        }

        aux2.volume = aux3.volume = aux4.volume = aux5.volume = 0;
    }
}