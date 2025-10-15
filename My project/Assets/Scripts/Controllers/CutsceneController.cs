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

    void Start()
    {
        // Detener con fade out la música de boot si está sonando
        if (BootMusicManager.Instance != null)
        {
            BootMusicManager.Instance.FadeOutMusic(1.5f);
        }

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
               // musicSource.Play();
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
        Debug.Log("Timeline finished!");

        // Desubscribe from the event to avoid multiple calls
        introTimeline.stopped -= OnTimelineFinished;

        if (dialogueController != null)
        {
            dialogueController.StartDialogue();
        }
    }

    public void StopIntro()
    {
        if (musicSource != null)
        {
          //  musicSource.Stop();
          StartCoroutine(AudioFader.FadeOutCoroutine(musicSource, 1.5f));
        }

        if (introTimeline != null)
        {
            introTimeline.Stop();
            introTimeline.stopped -= OnTimelineFinished;
        }
        else
        {
            Debug.LogWarning("Intro timeline is not assigned.");
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
        if (musicSource != null)
            StartCoroutine(AudioFader.FadeOutCoroutine(musicSource, 1f));
            
        StartCoroutine(LoadNextSceneAfterFade(1f));
    }

    private IEnumerator LoadNextSceneAfterFade(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("PrincipalMap");
    }
}
