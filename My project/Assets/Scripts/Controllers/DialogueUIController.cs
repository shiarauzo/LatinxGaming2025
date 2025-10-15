using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    public Button closeButton;
    private BaseNPC currentNPC;

    void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);
    }

    public void SetCurrentNPC(BaseNPC npc)
    {
        currentNPC = npc;
    }

    private void OnCloseClicked()
    {
        if (currentNPC != null)
        {
             currentNPC.EndDialogue();
        } else
        {
            Debug.LogWarning("⚠️ No hay NPC activo asignado al DialogueUIController.");
        }
    }
}