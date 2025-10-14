using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentalNPC : MonoBehaviour, IInteractable
{
    [Header("Planta asociada")]
    public int speciesIndex; // 0: Orchids, 1: CatsClaw, 2: Cacao

    [Header("Dialogue Data")]
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    // Variables temporales para dialogo actual
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
        // get name
        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        // GameController.Instance.PauseController.SetPause(true);

        // Obtener estado de diálogo según PlayerState.Plants
        var playerState = GetCurrentDialogueState();
        if (playerState == null)
        {
            Debug.LogWarning("No se encontró un estado de diálogo actual");
            EndDialogue();
            return;
        }

        currentDialogueLines = (PlayerPrefs.GetInt("Language", 0) == 0) ? playerState.englishLines : playerState.spanishLines;
        currentAutoProgressLines = playerState.autoProgressLines;
        currentTypingSpeed = playerState.typingSpeed;

        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
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
    private NPCDialogue.DialogueState GetCurrentDialogueState()
    {
        var states = dialogueData.dialogueStates;
        var player = GameController.Instance.playerState;
        var NPCSpecie = player.plantSpecies[speciesIndex];

        bool anyBurning = false;
        bool anyBurned = false;

        foreach (var parcel in NPCSpecie)
        {
            if (parcel.isBurning)
                anyBurning = true;
            else if (parcel.isBurned)
                anyBurned = true;
        }

        if (anyBurning)
            return System.Array.Find(states, s => s.stateName == "isBurning");
        if (anyBurned)
            return System.Array.Find(states, s => s.stateName == "isBurned");

        return System.Array.Find(states, s => s.stateName == "isRestored");
    }
}

