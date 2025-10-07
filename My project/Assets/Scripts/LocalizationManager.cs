using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, string> english = new();
    private Dictionary<string, string> spanish = new();
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTexts();
            Debug.Log("LocalizationManager inicializado y persistente");
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void LoadTexts()
    {
        // ES
        spanish["PLAY"] = "JUGAR";
        spanish["SETTINGS"] = "OPCIONES";
        spanish["CREDITS"] = "CRÉDITOS";

        // EN
        english["PLAY"] = "PLAY";
        english["SETTINGS"] = "SETTINGS";
        english["CREDITS"] = "CREDITS";
    }

    public string GetText(string key)
    {
        int lang = PlayerPrefs.GetInt("Language", 0);
        return lang == 0 ? (english.ContainsKey(key) ? english[key] : key) : (spanish.ContainsKey(key) ? spanish[key] : key);
    }
}
