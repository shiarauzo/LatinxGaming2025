using UnityEngine;

[System.Serializable]
public class PlantState
{
    public string plantNameES;
    public string plantNameEN;
    public bool isBurning = false; // Se está quemando ahora
    public bool isBurned = false; // Se quemó completamente

    public bool isRestored = false;  // Se replantó o curó → cuenta como salvada

    public string GetName()
    {
        bool isSpanish = PlayerPrefs.GetInt("Language", 0) == 1;
        return isSpanish ? plantNameES : plantNameEN;
    }
}