using UnityEngine;

[System.Serializable]
public class PlantState
{
    public string plantNameES;
    public string plantNameEN;
    public bool isBurning = false;
    public bool isBurned = false;

    public string GetName()
    {
        bool isSpanish = PlayerPrefs.GetInt("Language", 0) == 1;
        return isSpanish ? plantNameES : plantNameEN;
    }
}