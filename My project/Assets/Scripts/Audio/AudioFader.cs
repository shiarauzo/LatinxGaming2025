using System.Collections;
using System.Diagnostics;
using UnityEngine;

public static class AudioFader
{
    // Hace un fade in de un AudioSource, opcionalmente con un nuevo AudioClip.
    // useUnscaledTime = true -> usa Time.unscaledDeltaTime (útil mientras el juego está pausado)
    public static IEnumerator FadeInCoroutine(AudioSource audioSource, float duration, AudioClip newClip = null, bool useUnscaledTime = false)
    {
        if (audioSource == null)
            yield break;

        if (newClip != null)
            audioSource.clip = newClip;

        audioSource.volume = 0f;
        if (!audioSource.isPlaying)
            audioSource.Play();

        float targetVolume = 1f;
        float t = 0f;

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    // Hace un fade out suave y detiene el AudioSource.
    public static IEnumerator FadeOutCoroutine(AudioSource audioSource, float duration, bool useUnscaledTime = false)
    {
        if (audioSource == null)
            yield break;

        float startVolume = audioSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    // Transiciona de un track a otro con fade out + fade in.
    public static IEnumerator FadeToNewTrackCoroutine(AudioSource audioSource, AudioClip newClip, float duration, bool useUnscaledTime = false)
    {
        if (audioSource == null || newClip == null) yield break;

        float startVolume = audioSource.volume;
        float t = 0f;

        // Fade Out
        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade In
        t = 0f;
        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, startVolume, t / duration);
            yield return null;
        }

        audioSource.volume = startVolume;
    }
}