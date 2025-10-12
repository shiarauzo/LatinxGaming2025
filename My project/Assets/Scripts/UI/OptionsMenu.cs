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

    private void OnEnable()
    {
        // Cada vez que se abra el panel, sincroniza el toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }
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
        PlayerPrefs.Save();

        //Debug.Log("Pantalla completa: " + isFullscreen);
    }

    public void SetLanguage(int index)
    {
        LocalizationManager.Instance.SetLanguage(index);
       // Debug.Log("Idioma cambiado a: " + index);
    }

    public void CloseOptions()
    {
        gameObject.SetActive(false);
    }
}
