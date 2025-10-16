using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    public string key; //e.g. PLAY
    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    public void UpdateText()
    {
        if (textComponent != null && LocalizationManager.Instance != null)
        {
            textComponent.text = LocalizationManager.Instance.GetText(key);
        }
    }
    
    private void OnEnable()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
        
        UpdateText(); 
        // Suscribe event
        LocalizationManager.OnLanguageChanged += UpdateText;
    }

    private void OnDisable()
    {
        if(LocalizationManager.Instance != null)
            LocalizationManager.OnLanguageChanged -= UpdateText;
    }

}


