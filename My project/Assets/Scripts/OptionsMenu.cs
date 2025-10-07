using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Cargar valores previos
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        languageDropdown.value = PlayerPrefs.GetInt("Language", 0);

        // Aplicar configuraciones
        AudioListener.volume = volumeSlider.value;
        Screen.fullScreen = fullscreenToggle.isOn;

        // Listeners
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        languageDropdown.onValueChanged.AddListener(SetLanguage);
        
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetLanguage(int index)
    {
        // PlayerPrefs.SetInt("Language", index);
        // PlayerPrefs.Save();

        // Update all texts in LocalizedText 
        // foreach (var lt in FindObjectsOfType<LocalizedText>())
        // {
        //    lt.UpdateText();
        // }
        LocalizationManager.Instance.SetLanguage(index);
        Debug.Log("Idioma cambiado a: " + index);
    }

    public void CloseOptions()
    {
        gameObject.SetActive(false);
    }
}
