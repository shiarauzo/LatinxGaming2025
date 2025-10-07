using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    public string key; //e.g. PLAY
    private TMP_Text textComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance != null) {
            textComponent.text = LocalizationManager.Instance.GetText(key);
        }
    }
}
