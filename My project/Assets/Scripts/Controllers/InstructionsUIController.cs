using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsUIController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject instructionsPanel;
    public TMP_Text instructionText;

    public void ShowInstructions(string key)
    {
        if (instructionText != null)
            instructionText.text = LocalizationManager.Instance.GetText(key);;

        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }

    public void CloseInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }

}