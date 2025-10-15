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

    // Variables temporales para dialogo actual
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private string[] currentDialogueLines;
    private bool[] currentAutoProgressLines;
    private float currentTypingSpeed;

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
    
        // Obtener estado de diálogo según PlayerState.Plants
        var state = GetCurrentDialogueState();
        if (state == null)
        {
            EndDialogue();
            return;
        }

        currentDialogueLines = (PlayerPrefs.GetInt("Language", 0) == 0) ? state.englishLines : state.spanishLines;
        currentAutoProgressLines = state.autoProgressLines;
        currentTypingSpeed = state.typingSpeed;

        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isTyping = false;
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
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

        foreach (char letter in currentDialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(currentTypingSpeed); // Works even if Game is Paused.
        }

        isTyping = false;

        if (currentAutoProgressLines.Length > dialogueIndex && currentAutoProgressLines[dialogueIndex])
        {
            yield return new WaitForSecondsRealtime(dialogueData.dialogueStates[0].autoProgressDelay);
            NextLine();
        }
    }

    // Método que será implementado en las clases derivadas
    protected abstract NPCDialogue.DialogueState GetCurrentDialogueState();
}