using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
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
        Debug.Log($"NPC.Interact() ejecutado en {name}");
        Debug.Log($"dialogueData: {(dialogueData == null ? "null" : "ok")}");

        if (dialogueData == null || (GameController.Instance.PauseController.IsPaused && !isDialogueActive))
            return;

        if (isDialogueActive)
            NextLine();
        else
            StartDialogue();
    }

    void StartDialogue()
    {
        Debug.Log("starting dialogue");
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        GameController.Instance.PauseController.SetPause(true);

        // Obtener estado de diálogo según PlayerState
        var playerState = GetCurrentDialogueState();  //GameController.Instance.playerState;
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

    void NextLine()
    {
        if (isTyping)
        {
            // Completa la línea actual
            StopAllCoroutines();
            dialogueText.SetText(currentDialogueLines[dialogueIndex]);
            isTyping = false;
        } else if(++dialogueIndex < currentDialogueLines.Length)
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
          //  yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (currentAutoProgressLines.Length > dialogueIndex && currentAutoProgressLines[dialogueIndex])
        {
            yield return new WaitForSecondsRealtime(dialogueData.dialogueStates[0].autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        GameController.Instance.PauseController.SetPause(false);
    }
    
    // Determina el estado de diálogo actual según PlayerState
    private NPCDialogue.DialogueState GetCurrentDialogueState()
    {
        var states = dialogueData.dialogueStates;
        var player = GameController.Instance.playerState;

        // First Encounter
        if (!player.hasCollectedSeeds && !player.burnedPlot && !player.restoredPlot)
            return System.Array.Find(states, s => s.stateName == "FirstEncounter");

        // Initial Reminder
        if (!player.hasCollectedSeeds && !player.burnedPlot)
            return System.Array.Find(states, s => s.stateName == "InitialReminder");

        // Partial Reminder
        if (!player.hasCollectedSeeds && player.burnedPlot && !player.restoredPlot)
            return System.Array.Find(states, s => s.stateName == "PartialReminder");

        // Fire Alert
        if (GameController.Instance.playerState.plants != null &&
            System.Array.Exists(GameController.Instance.playerState.plants, p => p.isBurning))
        {
            return System.Array.Find(states, s => s.stateName == "FireAlert");
        }

        // Burned Plot + Seeds
        if (player.burnedPlot && player.hasCollectedSeeds)
            return System.Array.Find(states, s => s.stateName == "BurnedPlotWithSeeds");

        // Burned Plot + No Seeds
        if (player.burnedPlot && !player.hasCollectedSeeds)
            return System.Array.Find(states, s => s.stateName == "BurnedPlotNoSeeds");

        // Restored Plot
        if (player.restoredPlot)
            return System.Array.Find(states, s => s.stateName == "RestoredPlot");

        // Plot not yet restored
        if (player.burnedPlot && !player.restoredPlot)
            return System.Array.Find(states, s => s.stateName == "PlotNotYetRestored");

        // Default to First Encounter if no conditions match
        return System.Array.Find(states, s => s.stateName == "FirstEncounter");
    }
}