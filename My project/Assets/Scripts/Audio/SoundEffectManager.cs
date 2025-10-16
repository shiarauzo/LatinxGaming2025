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

    public static void PlayLongSFX(string soundName)
    {
        AudioClip clip = soundEffectLibrary.GetRandomClip(soundName);
        if (clip == null) return;

        if (longSfxSource.isPlaying && longSfxSource.clip == clip)
            return;

        longSfxSource.clip = clip;
        longSfxSource.Play();


    }
    
    public static void StopLongSFX()
    {
        if (longSfxSource != null && longSfxSource.isPlaying)
            longSfxSource.Stop();
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