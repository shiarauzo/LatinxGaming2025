using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Plant
{
    public GameObject plantObject;
    public bool isBurning = false;
    public bool isBurned = false;
}

public class Level1Controller : MonoBehaviour
{
    public Plant[] plants;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartRandomFire());
    }

    private IEnumerator StartRandomFire()
    {
        while(true)
        {
            float waitTime = Random.Range(15f, 30f);
            yield return new WaitForSeconds(waitTime);

            BurnRandomPlant();
        }
    }

    private void BurnRandomPlant()
    {
        // Verificar si ya hay una planta quemándose
        if (System.Array.Exists(plants, p => p.isBurning))
        {
            // Debug.Log("Ya hay una planta quemándose. Esperando...");
            return;
        }

        var readyToBurnPlants = new List<Plant>();
        foreach (var plant in plants)
        {
            if (!plant.isBurned && !plant.isBurning)
                readyToBurnPlants.Add(plant);
        }

        if (readyToBurnPlants.Count == 0)
        {
            Debug.Log("Todas las plantas ya están quemadas o en proceso.");
            return;
        }

        int randomIndex = Random.Range(0, readyToBurnPlants.Count);
        Plant burningPlant = readyToBurnPlants[randomIndex];

        burningPlant.isBurning = true;
        burningPlant.isBurned = false;

        // Sincronizar con PlayerState
        PlayerState ps = GameController.Instance.playerState;
        PlantState statePlant = ps.plants[randomIndex];
        statePlant.isBurning = true;
        statePlant.isBurned = false;

        // Debug.Log($"🔥 La planta {statePlant.GetName()} empezó a quemarse!");

        // Notificar al GameController que hay fuego activo
        GameController.Instance.playerState.isAnyPlantBurning = true;


        StartCoroutine(FinishBurningPlant(burningPlant, statePlant));
    }

    // Después de 90 segundos, pasa a quemada
    private IEnumerator FinishBurningPlant(Plant plant, PlantState statePlant)
    {
        yield return new WaitForSeconds(10f);

        plant.isBurning = false;
        plant.isBurned = true;
        // Debug.Log($"💀 La planta {statePlant.GetName()} se ha quemado completamente.");

        // Verificar si aún hay plantas quemándose
        GameController.Instance.playerState.isAnyPlantBurning = System.Array.Exists(plants, p => p.isBurning);
        // Actualizar burnedPlot si al menos una planta está quemada
        GameController.Instance.playerState.burnedPlot = System.Array.Exists(plants, p => p.isBurned);

        CheckWinLose();
    }
    
    public void CheckWinLose()
    {
        var ps = GameController.Instance.playerState;

        bool allSaved = System.Array.TrueForAll(ps.plants, p => p.isRestored);
        bool allBurned = System.Array.TrueForAll(ps.plants, p => p.isBurned);

        if (allSaved)
        {
            Debug.Log("🎉 ¡Has salvado todas las plantas! ¡Ganaste!");
            // Lógica para manejar la victoria
            // TODO: cargar escena de victoria, audio VICTORY
            // SceneManager.LoadScene("VictoryScene");
        }
        else if (allBurned)
        {
            Debug.Log("💀 Todas las plantas se han quemado. Has perdido.");
            // Lógica para manejar la derrota
            // TODO: cargar escena de LOSE, audio LOSE
            // SceneManager.LoadScene("GameOverScene");
        }
    }
}
