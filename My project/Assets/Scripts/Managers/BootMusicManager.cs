using System.Collections;
using UnityEngine;

public class BootMusicManager : MonoBehaviour
{
    public static BootMusicManager Instance;
    public AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // Fade out música global
    public void FadeOutMusic(float duration = 1.5f)
    {
        if (musicSource != null)
            StartCoroutine(AudioFader.FadeOutCoroutine(musicSource, duration));
    }

    // Fade in música global, opcional con nuevo clip   
    public void FadeInMusic(float duration = 1.5f, AudioClip audioClip = null)
    {
        if (musicSource != null)
            StartCoroutine(AudioFader.FadeInCoroutine(musicSource, duration, audioClip));
    }

    // Cambiar de track con fade out + fade in
    public void FadeToNewTrack(AudioClip newClip, float duration = 1.5f)
    {
        if (musicSource != null && newClip != null && musicSource.clip != newClip)
            StartCoroutine(AudioFader.FadeToNewTrackCoroutine(musicSource, newClip, duration));
    }
    
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}
