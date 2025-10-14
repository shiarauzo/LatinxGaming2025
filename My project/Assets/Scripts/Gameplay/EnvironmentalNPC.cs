using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentalNPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    public bool isBurning { get; private set; }
    public bool isBurned { get; private set; }
    public bool isRestored { get; private set; }
    
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        // GameController.Instance.PauseController.SetPause(true);

    }
}
