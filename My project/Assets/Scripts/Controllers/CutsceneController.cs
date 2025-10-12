using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    public AudioSource musicSource;
    public PlayableDirector introTimeline;
    public DialogueController dialogueController;

    private bool wasTimelinePlaying = false;

    void Start()
    {
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
                musicSource.Play();
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
            musicSource.Stop();
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

    private void HandlePauseChanged(bool isPaused)
    {
        SetPauseTimeline(isPaused);
        SetPauseMusic(isPaused);

        if (dialogueController != null)
            dialogueController.SetPauseDialogue(isPaused);
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

}
