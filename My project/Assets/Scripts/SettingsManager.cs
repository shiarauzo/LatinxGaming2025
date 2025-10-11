using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("UI Panel")]
    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
        }
    }
}
