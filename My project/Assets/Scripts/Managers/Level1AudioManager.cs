using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Level1AudioManager: MonoBehaviour
{
    [Header("Start Segments")]
    public AudioSource start1, start2, start3, start4, start5;
    [Header("Loop Segments")]
    public AudioSource loop1, loop2, loop3, loop4, loop5;

    [Header("Cues")]
    public AudioSource victoryCue, loseCue;

    private AudioSource[] startTracks;
    private AudioSource[] loopTracks;
    private Coroutine[] fadeCoroutines;
    private int intensityLevel = 0;
    private bool startsFinished = false;


    void Start()
    {
        startTracks = new[] { start1, start2, start3, start4, start5 };
        loopTracks = new[] { loop1, loop2, loop3, loop4, loop5 };
        fadeCoroutines = new Coroutine[5];

        for (int i = 0; i < 5; i++)
        {
            startTracks[i].volume = (i == 0) ? 1f : 0f;
            startTracks[i].loop = false;

            loopTracks[i].volume = 0f;
            loopTracks[i].loop = true;
        }

        double dspTime = AudioSettings.dspTime + 0.1; // empezar 0.1s en el futuro
        foreach (var s in startTracks)
            s.PlayScheduled(dspTime);

        float maxStartLength = start1.clip != null ? start1.clip.length : 0f;
        StartCoroutine(StartLoopAfterDelay(maxStartLength));
    }
    
    void Update()
{
    for (int i = 0; i < startTracks.Length; i++)
    {
        if (startTracks[i].isPlaying)
            Debug.Log($"StartTrack {i+1} isPlaying: {startTracks[i].isPlaying}, volume: {startTracks[i].volume}");
    }

    for (int i = 0; i < loopTracks.Length; i++)
    {
        if (loopTracks[i].isPlaying)
            Debug.Log($"LoopTrack {i+1} isPlaying: {loopTracks[i].isPlaying}, volume: {loopTracks[i].volume}");
    }
}


    private IEnumerator StartLoopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        startsFinished = true;

        // Apagar todos los starts
        foreach (var s in startTracks)
            s.Stop();
        
        // Preparar loopTracks (ya configurados) y reproducir todos al mismo tiempo
         foreach (var l in loopTracks)
        {
            l.volume = 0f;
            l.Play();
        }
    }
    
    public void UpdateIntensity(int burningParcels)
    {
        int newLevel = 0;
        if (burningParcels >= 8) newLevel = 4;
        else if (burningParcels >= 6) newLevel = 3;
        else if (burningParcels >= 4) newLevel = 2;
        else if (burningParcels >= 1) newLevel = 1;

        if (newLevel <= intensityLevel) return;
        intensityLevel = newLevel;

        for (int i = 1; i <= intensityLevel; i++)
        {
            AudioSource target = startsFinished ? loopTracks[i] : startTracks[i];
            FadeAudio(target, 1f, i);
        }
    }
    
    private void FadeAudio(AudioSource source, float targetVolume, int index, float duration = 1f)
    {
        if (fadeCoroutines[index] != null)
            StopCoroutine(fadeCoroutines[index]);
        fadeCoroutines[index] = StartCoroutine(FadeToVolume(source, targetVolume, duration));
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

    public void FadeOutAll(float duration = 1.5f)
    {
        StartCoroutine(FadeOutAllRoutine(duration));
    }
    
    private IEnumerator FadeOutAllRoutine(float duration)
    {
        float[] startVols = new float[loopTracks.Length];
        for (int i = 0; i < loopTracks.Length; i++)
            startVols[i] = loopTracks[i].volume;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = 1f - (t / duration);
            for (int i = 0; i < loopTracks.Length; i++)
                loopTracks[i].volume = startVols[i] * factor;
            yield return null;
        }

        foreach (var s in loopTracks)
            s.volume = 0f;
    }

    public void PlayVictory()
    {
        FadeOutAll(1f);
        victoryCue.Play();
    }
    public void PlayLose()
    {
        FadeOutAll(1f);
        loseCue.Play();
    }

    public void FadeOutAuxTracksExceptBackground(float duration = 1.5f)
    {
        for (int i = 1; i < loopTracks.Length; i++)
        {
            FadeAudio(loopTracks[i], 0f, i, duration);
        }
    }
}