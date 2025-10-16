using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseNPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    [Header("Voice Settings")]
    public AudioSource voiceSource;

    // Variables temporales para dialogo actual
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private string[] currentDialogueLines;
    private bool[] currentAutoProgressLines;
    private float currentTypingSpeed;

    private NPCDialogue.DialogueState currentState;
    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null || (GameController.Instance.PauseController.IsPaused && !isDialogueActive))
            return;

        if (isDialogueActive)
            NextLine();
        else
            StartDialogue();
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        currentState = GetCurrentDialogueState();
        if (currentState == null)
        {
            Debug.LogWarning("No se encontró DialogueState para este NPC");
            EndDialogue();
            return;
        }

        // For Testing without LocalizationManager
        if (LocalizationManager.Instance != null)
        {
            nameText.SetText(LocalizationManager.Instance.GetText(dialogueData.npcNameKey));
        }
        else
        {
            Debug.LogWarning("⚠️ LocalizationManager no encontrado. Usando key temporal.");
            nameText.SetText(dialogueData.npcNameKey);
        }

        portraitImage.sprite = dialogueData.npcPortrait;

        // Mostrar panel
        dialoguePanel.SetActive(true);

        // Vincular el NPC al UIController
        var ui = dialoguePanel.GetComponent<DialogueUIController>();
        if (ui != null)
            ui.SetCurrentNPC(this);

        currentDialogueLines = (PlayerPrefs.GetInt("Language", 0) == 0) ? currentState.englishLines : currentState.spanishLines;
        currentAutoProgressLines = currentState.autoProgressLines;
        currentTypingSpeed = currentState.typingSpeed;

        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isTyping = false;
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        if (voiceSource != null && voiceSource.isPlaying)
        {
            StartCoroutine(FadeOutVoice(0.5f));
            //voiceSource.Stop();
            voiceSource.loop = false;
        }
    }

    void NextLine()
    {
        if (isTyping)
        {
            // Completa la línea actual
            StopAllCoroutines();
            dialogueText.SetText(currentDialogueLines[dialogueIndex]);
            isTyping = false;
        } else if (++dialogueIndex < currentDialogueLines.Length)
        {
            // Si hay más líneas, tipeamos la siguiente
            StartCoroutine(TypeLine());
        } else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        AudioClip currentClip = null;

        if (currentState != null)
        {
            if (currentState.useSingleVoiceClip)
            {
                currentClip = currentState.voiceSound;
            }
            else
            {
                if (PlayerPrefs.GetInt("Language", 0) == 0 && currentState.englishVoiceClips.Length > dialogueIndex)
                    currentClip = currentState.englishVoiceClips[dialogueIndex];
                else if (currentState.spanishVoiceClips.Length > dialogueIndex)
                    currentClip = currentState.spanishVoiceClips[dialogueIndex];
            }
        }
        
        if (currentClip != null && voiceSource != null)
        {
            voiceSource.Stop();
            voiceSource.clip = currentClip;
            if (currentState.useSingleVoiceClip)
            {
                voiceSource.loop = true;
            }  else
            {
                voiceSource.loop = false;
            }
            
            voiceSource.Play();
        }

        foreach (char letter in currentDialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(currentTypingSpeed); // Works even if Game is Paused.
        }

        isTyping = false;
        
        if (currentState.useSingleVoiceClip && voiceSource != null && voiceSource.isPlaying)
        {
            voiceSource.Stop();
            voiceSource.loop = false;
        }

        if (currentAutoProgressLines.Length > dialogueIndex && currentAutoProgressLines[dialogueIndex])
        {
            yield return new WaitForSecondsRealtime(dialogueData.dialogueStates[0].autoProgressDelay);
            NextLine();
        }
    }

    IEnumerator FadeOutVoice(float duration)
    {
        if (voiceSource == null) yield break;

        float startVolume = voiceSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            voiceSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        voiceSource.Stop();
        voiceSource.volume = startVolume;
    }

    // Método que será implementado en las clases derivadas
    protected abstract NPCDialogue.DialogueState GetCurrentDialogueState();
}