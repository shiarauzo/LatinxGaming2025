using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour {
    private static SoundEffectManager Instance;
    private static AudioSource audioSource;
    private static AudioSource longSfxSource;
    private static SoundEffectLibrary soundEffectLibrary;
    private static string currentLongSFX;
    //[SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            audioSource = GetComponent<AudioSource>();
            longSfxSource = GetComponent<AudioSource>();
           // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static void Play(string soundName)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    public static void PlayLongSFX(string sfxName)
    {
        AudioClip clip = soundEffectLibrary.GetRandomClip(sfxName);
        if (clip == null) return;

        if (longSfxSource == null)
            longSfxSource = audioSource;

        longSfxSource.clip = clip;
        longSfxSource.loop = true;
        longSfxSource.Play();
        currentLongSFX = sfxName;
    }
    
    public static void StopLongSFX(string sfxName)
    {
        if (longSfxSource != null && longSfxSource.isPlaying && currentLongSFX == sfxName)
        {
            longSfxSource.Stop();
            longSfxSource.loop = false;
            currentLongSFX = null;
        }
    }

/*     void Start()
    {
       // sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    } */

/*     public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    } */
/*     public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    } */
}