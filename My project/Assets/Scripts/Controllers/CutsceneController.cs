using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public AudioSource musicSource;
    public PlayableDirector introTimeline;
    public IntroDialogueController dialogueController;

    private bool wasTimelinePlaying = false;
    private bool introSkipped = false;

    void Start()
    {
        // Detener con fade out la música de boot si está sonando
        if (GlobalMusicManager.Instance != null)
            GlobalMusicManager.Instance.FadeOutMusic(1.5f);

        if (dialogueController != null)
            dialogueController.OnDialogueFinished += GoToNextScene;
        PlayIntro();
    }

    public void PlayIntro()
    {
        if (introTimeline != null)
        {
            introTimeline.stopped += OnTimelineFinished; // suscribe to the stopped event
            introTimeline.Play();
            if (musicSource != null)
            {
                musicSource.loop = true;
                StartCoroutine(AudioFader.FadeInCoroutine(musicSource, 1.5f));
            }
        }
        else
        {
            Debug.LogWarning("Intro timeline is not assigned.");
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        // Desubscribe from the event to avoid multiple calls
        introTimeline.stopped -= OnTimelineFinished;

        if (!introSkipped && dialogueController != null)
        {
            dialogueController.StartDialogue();
        }
    }

    public void SkipIntro()
    {
        introSkipped = true;
        StopIntro();
        GoToNextScene();
    }

    public void StopIntro()
    {
        if (musicSource != null)
          StartCoroutine(AudioFader.FadeOutCoroutine(musicSource, 1.5f));

        if (introTimeline != null)
        {
            introTimeline.Stop();
            introTimeline.stopped -= OnTimelineFinished;
        }
    }

    public void SetPauseTimeline(bool pause)
    {
        if (introTimeline == null) return;

        if (pause)
        {
            wasTimelinePlaying = introTimeline.state == PlayState.Playing;
            introTimeline.Pause();
        }
        else {
            if (wasTimelinePlaying)
                introTimeline.Play();
        }
    }

    public void SetPauseMusic(bool pause)
    {
        if (musicSource == null) return;

        if (pause && musicSource.isPlaying)
            musicSource.Pause();
        else if (!pause)
            musicSource.UnPause();
    }

    public void GoToNextScene()
    {
        Debug.Log($"[CutsceneController] GoToNextScene() called at {Time.time}");

        if (GlobalUIManager.Instance != null)
            GlobalUIManager.Instance.FadeScreen(true, 0.5f);
        
        if (GlobalMusicManager.Instance != null)
        {
            GlobalMusicManager.Instance.FadeOutAndLoadScene("PrincipalMap", 1f);
        }
        else
        {
            StartCoroutine(FadeAndLoadFallback());
        }
    }

    private IEnumerator FadeAndLoadFallback()
    {
        Debug.Log($"[CutsceneController] Fade start (fallback) at {Time.time}");
        if (musicSource != null)
            yield return StartCoroutine(AudioFader.FadeOutCoroutine(musicSource, 1f));

        UnityEngine.SceneManagement.SceneManager.LoadScene("PrincipalMap");
        Debug.Log($"[CutsceneController] Fade end (fallback) at {Time.time}");
    }
}
