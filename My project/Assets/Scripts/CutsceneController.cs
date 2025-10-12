using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    public AudioSource musicSource;
    public PlayableDirector introTimeline;
    public DialogueController dialogueController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLanguageData();
        PlayIntro();
    }

    void LoadLanguageData()
    {
        // Placeholder for loading language data
        Debug.Log("Language data loaded.");
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
}
