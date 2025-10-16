// Assets/Scripts/Controllers/IntroDialogueController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string text;
    public AudioClip voice;
}

[System.Serializable]
public class DialogueData
{
    public DialogueLine[] spanishLines;
    public DialogueLine[] englishLines;
}
public class IntroDialogueController : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public DialogueData dialogueData;
    public AudioSource voiceSource;
    public UnityEngine.Audio.AudioMixerGroup voicesGroup;


    public enum Language { English, Spanish }
    public Language currentLanguage = Language.English;

    private DialogueLine[] currentLines;
    private int currentLineIndex = 0;

    private Coroutine typingCoroutine;
    private Coroutine continueCoroutine;

    [Header("Continue Text")]
    public TMP_Text continueText;
    public string continueTextEnglish = "Press Enter to continue";
    public string continueTextSpanish = "Presiona Enter para continuar";

    public bool loadNextSceneOnEnd = true;

    private enum LineState { Typing, Completed, Waiting }
    private LineState lineState = LineState.Completed;

    public delegate void DialogueFinished();
    public event DialogueFinished OnDialogueFinished;

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
    }

    void Awake()
    {
        if (dialogueText != null)
            dialogueText.text = "";

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continueText != null)
            continueText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Next line on Enter key
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            HandleEnter();
        }    
    }

    public void StartDialogue()
    {
        // Read language stored en PlayerPrefs (0 = English, 1 = Spanish)
        int langIndex = PlayerPrefs.GetInt("Language", 0);
        currentLanguage = (langIndex == 0) ? Language.English : Language.Spanish;
        
        if (voiceSource != null)
            voiceSource.outputAudioMixerGroup = voicesGroup;

        // Show the dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        currentLines = currentLanguage == Language.Spanish ? dialogueData.spanishLines : dialogueData.englishLines;

        if (currentLines == null || currentLines.Length == 0)
        {
            Debug.LogWarning("⚠️ No hay líneas disponibles para el idioma actual: " + currentLanguage);
            return;
        }

        currentLineIndex = 0;
        ShowNextLine();
    }

    private void HandleEnter()
    {
        switch (lineState)
        {
            case LineState.Typing:
                // Skip typewriter + audio
                SkipCurrentLine();
                break;
            case LineState.Completed:
                // Enter after show complete → move to next line
                ShowNextLine();
                break;
            case LineState.Waiting:
                // Waiting: ignore input
                break;
        }
    }
    private void ShowNextLine()
    {
        // Hide continue text
        if (continueText != null)
            continueText.gameObject.SetActive(false);

        if (currentLines == null || currentLines.Length == 0)
        {
            Debug.LogWarning("⚠️ currentLines está vacío");
            return;
        }
    
        // If no more lines, end dialogue
        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentLines[currentLineIndex];
        currentLineIndex++;

        // Start typing
        typingCoroutine = StartCoroutine(TypeLine(line));
        lineState = LineState.Typing;
    }


    private void SkipCurrentLine()
    {
       // Debug.Log("Skipping to full line");
       // Debug.Log("Current lines idx: " + currentLineIndex);
        // Stop typing and show full line
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // Stop voice if playing
        if (voiceSource != null && voiceSource.isPlaying)
            voiceSource.Stop();

        // Show all text
        DialogueLine line = currentLines[currentLineIndex - 1];
        dialogueText.text = line.text;

        // Show continue text
        ShowContinueText();
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        dialogueText.text = "";

        // Reproducir voz
        if (line.voice != null && voiceSource != null)
            SetupVoice(line.voice);

        foreach (char letter in line.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;

        // Wait a moment before showing continue text
        if (voiceSource != null && line.voice != null)
        {
            yield return null;
        }

        ShowContinueText();
    }
    
    private void ShowContinueText()
    {
        if (continueCoroutine != null)
            StopCoroutine(continueCoroutine);

        continueCoroutine = StartCoroutine(ShowContinueAfterDelay(0.5f));
    }
    private IEnumerator ShowContinueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
            continueText.text = currentLanguage == Language.Spanish ? continueTextSpanish : continueTextEnglish;
        }

         lineState = LineState.Completed;
    }

    private void EndDialogue()
    {
        // Hide the dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Disparar evento
        OnDialogueFinished?.Invoke();
    }

    // Cuando cambia de idioma, recargar diálogo
    private void OnLanguageChanged()
    {
        if (dialogueData == null || currentLines == null)
            return;

        Debug.Log("🌐 Idioma cambiado — recargando diálogo actual");

        if (voiceSource != null)
            voiceSource.outputAudioMixerGroup = voicesGroup;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (voiceSource != null)
            voiceSource.Stop();

        int langIndex = PlayerPrefs.GetInt("Language", 0);
        currentLanguage = (langIndex == 0) ? Language.English : Language.Spanish;
        currentLines = currentLanguage == Language.Spanish ? dialogueData.spanishLines : dialogueData.englishLines;

        // Reiniciar el dialogo actual desde la línea activa
        int restartIndex = Mathf.Clamp(currentLineIndex - 1, 0, currentLines.Length - 1);
        currentLineIndex = restartIndex;
        ShowNextLine();
    }

    private void SetupVoice(AudioClip clip)
    {
        if (voiceSource == null || clip == null) return;

        voiceSource.outputAudioMixerGroup = voicesGroup;
        voiceSource.volume = 1f;      // volumen uniforme
        voiceSource.panStereo = 0f;
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void SetPauseDialogue(bool pause)
    {
        if (pause)
        {
            if (voiceSource != null && voiceSource.isPlaying)
                voiceSource.Pause();

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
        }
        else
        {
            if (voiceSource != null && voiceSource.clip != null)
                voiceSource.UnPause();

            if (currentLineIndex > 0 && currentLineIndex <= currentLines.Length)
            {
                DialogueLine line = currentLines[currentLineIndex - 1];
                typingCoroutine = StartCoroutine(TypeLine(line));
            }
        }
    }
}
