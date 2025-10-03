using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector introTimeline;

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
            introTimeline.Play();
        }
        else
        {
            Debug.LogWarning("Intro timeline is not assigned.");
        }
    }

    public void StopIntro()
    {
        if (introTimeline != null)
        {
            introTimeline.Stop();
        }
        else
        {
            Debug.LogWarning("Intro timeline is not assigned.");
        }
    }
}
