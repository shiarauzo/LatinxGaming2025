using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
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

    [Header("Internal States")]
    public bool firstEncounter = true;
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

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
       // GameController.Instance.PauseController.SetPause(true);

        // Obtener estado de diálogo según PlayerState
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

        // Prioridad al fuego, incluso en la primera interacción
        if (player.isAnyPlantBurning)
            return System.Array.Find(states, s => s.stateName == "FireAlert");


        // First Encounter
        if (firstEncounter)
        {
            firstEncounter = false;
            return System.Array.Find(states, s => s.stateName == "FirstEncounter");
        }
            
        // Initial Reminder: no ha recolectado semillas ni apagado incendios
        if (!player.hasCollectedSeeds && !player.hasExtinguishedFire && !player.burnedPlot)
            return System.Array.Find(states, s => s.stateName == "InitialReminder");

        // Burned Plot + Seeds: terreno quemado y jugador tiene semillas
        if (player.burnedPlot && player.hasCollectedSeeds)
            return System.Array.Find(states, s => s.stateName == "BurnedPlotWithSeeds");

        // Burned Plot + No Seeds: terreno quemado y jugador no tiene semillas
        if (player.burnedPlot && !player.hasCollectedSeeds)
            return System.Array.Find(states, s => s.stateName == "BurnedPlotNoSeeds");

        // Restored Plot: terreno/planta restaurado
        if (player.restoredPlot)
            return System.Array.Find(states, s => s.stateName == "RestoredPlot");

        // Plot not yet restored: terreno quemado pero no restaurado
        if (player.burnedPlot && !player.restoredPlot)
            return System.Array.Find(states, s => s.stateName == "PlotNotYetRestored");
            
        // Partial Reminder: catch-all si ninguna otra condición se cumple
        return System.Array.Find(states, s => s.stateName == "PartialReminder");
    }
}