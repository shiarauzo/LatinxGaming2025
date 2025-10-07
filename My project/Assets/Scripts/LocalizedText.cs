using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    public string key; //e.g. PLAY
    private TMP_Text textComponent;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        UpdateText();
    }

    public void UpdateText()
    {
        var tmp = GetComponent<TMP_Text>();
        
        if (tmp != null && LocalizationManager.Instance != null)
        {
            tmp.text = LocalizationManager.Instance.GetText(key);
        }
    }
}
