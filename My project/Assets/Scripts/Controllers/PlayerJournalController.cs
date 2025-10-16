using UnityEngine;
using UnityEngine.UI;

public class PlayerJournalController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject journalUI;
    public Button openButton;
    public Button closeButton;

    [Header("Audio")]
    public AudioSource journalMusicSource; 
    public AudioClip journalMusicClip;
    public float fadeDuration = 1.5f;

    private bool isOpen = false;
    private Coroutine currentFadeCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        journalUI.SetActive(false);
        openButton.interactable = true;
        closeButton.gameObject.SetActive(false);

        openButton.onClick.AddListener(OpenJournal);
        closeButton.onClick.AddListener(CloseJournal);

        // AudioSource seguirá sonando aunque el juego esté pausado
        if (journalMusicSource != null)
            journalMusicSource.ignoreListenerPause = true;
    }

    // Update is called once per frame
    public void OpenJournal()
    {
        if (isOpen) return;

        GameController.Instance.PauseController.SetPause(true);
        isOpen = true;
        journalUI.SetActive(true);
        openButton.interactable = false;
        closeButton.gameObject.SetActive(true);

        if (journalMusicSource != null && journalMusicClip != null)
        {
            if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = StartCoroutine(AudioFader.FadeInCoroutine(journalMusicSource, fadeDuration, journalMusicClip, true));
        }
    }
    
    public void CloseJournal()
    {
        if (!isOpen) return;

        GameController.Instance.PauseController.SetPause(false);
        isOpen = false;
        journalUI.SetActive(false);
        openButton.interactable = true;
        closeButton.gameObject.SetActive(false);

        if (journalMusicSource != null)
        {
            if (currentFadeCoroutine != null) StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = StartCoroutine(AudioFader.FadeOutCoroutine(journalMusicSource, fadeDuration, true));
        }
    }
}
