using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
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
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void FadeToNewTrack(AudioClip newClip, float duration = 1.5f)
    {
        if (musicSource.clip == newClip)
            return;
        StartCoroutine(FadeAndChange(newClip, duration));
    }
    
    public void FadeOutMusic(float duration = 1.5f)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = musicSource.volume;
        float t = 0;

        while (t < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = startVolume;
        musicSource.Stop();
    }

    private IEnumerator FadeAndChange(AudioClip newClip, float duration)
    {
        float startVolume = musicSource.volume;
        float t = 0f;

        // Fade out
        while (t < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        t = 0F;
        while (t < duration)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = startVolume;
    }
}
