using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class LocalizatiionEntry
{
    public string key;
    public string value;
}

[System.Serializable]
public class LocalizationDictionary
{
    public LocalizatiionEntry[] entries;

    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>();
        if (entries == null) return dict;
        foreach (var e in entries)
        {
            if (!string.IsNullOrEmpty(e.key))
                dict[e.key] = e.value ?? "";
        }

        return dict;
    }
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    public static event System.Action OnLanguageChanged;

    private Dictionary<string, string> english = new();
    private Dictionary<string, string> spanish = new();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTextsFromResources();
            Debug.Log("LocalizationManager inicializado y persistente");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void LoadTextsFromResources()
    {
        var engAsset = Resources.Load<TextAsset>("Localization/english");
        var spaAsset = Resources.Load<TextAsset>("Localization/spanish");

        if (engAsset != null)
        {
            var eng = JsonUtility.FromJson<LocalizationDictionary>(engAsset.text);
            english = eng.ToDictionary();
        }
        else
        {
            Debug.LogWarning("No se encontró Resources/Localization/english.json");
        }

        if (spaAsset != null)
        {
            var spa = JsonUtility.FromJson<LocalizationDictionary>(spaAsset.text);
            spanish = spa.ToDictionary();
        }
        else
        {
            Debug.LogWarning("No se encontró Resources/Localization/spanish.json");
        }

    }

    public string GetText(string key)
    {
        int lang = PlayerPrefs.GetInt("Language", 0); // // 0 = EN, 1 = ES
        var dict = (lang == 0) ? english : spanish;
        return dict.ContainsKey(key) ? dict[key] : key;
    }

    public void SetLanguage(int index)
    {
        PlayerPrefs.SetInt("Language", index);
        PlayerPrefs.Save();

        OnLanguageChanged?.Invoke();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstanceExists()
    {
        if (Instance == null)
        {
            var existing = FindAnyObjectByType<LocalizationManager>();
            if (existing == null)
            {
                var go = new GameObject("LocalizationManager_Auto");
                go.AddComponent<LocalizationManager>();
                Debug.Log("🌐 LocalizationManager creado automáticamente.");
            }
        }
    }
}
