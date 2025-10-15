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
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
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
}
