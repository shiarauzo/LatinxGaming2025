
using System.IO;
using UnityEngine;
using Unity.Cinemachine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Define Save Location
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        LoadGame();

    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = FindObjectOfType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            FindObjectOfType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();
        } else
        {
            SaveGame();
        }
    }

}
