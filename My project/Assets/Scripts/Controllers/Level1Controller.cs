using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Plant
{
    public string plantName;
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
        float waitTime = Random.Range(15f, 30f);
        yield return new WaitForSeconds(waitTime);

        BurnRandomPlant();
    }

    private void BurnRandomPlant()
    {
        var unburnedPlants = new List<Plant>();
        foreach (var plant in plants)
        {
            if (!plant.isBurned && !plant.isBurning)
                unburnedPlants.Add(plant);
        }

        if (unburnedPlants.Count == 0)
        {
            Debug.Log("Todas las plantas ya están quemadas o en proceso.");
            return;
        }

        int randomIndex = Random.Range(0, unburnedPlants.Count);
        Plant burningPlant = unburnedPlants[randomIndex];
        burningPlant.isBurned = true;

        Debug.Log($"🔥 La planta {burningPlant.plantName} empezó a quemarse!");

        // Notificar al GameController que hay fuego activo
        GameController.Instance.playerState.isAnyPlantBurning = true;

        // Después de 10 segundos, pasa a quemada
        StartCoroutine(FinishBurningPlant(burningPlant));

        // Reiniciar la rutina para quemar otra planta
        StartCoroutine(StartRandomFire());
    }

    private IEnumerator FinishBurningPlant(Plant plant)
    {
        yield return new WaitForSeconds(10f);

        plant.isBurning = false;
        plant.isBurned = true;
        Debug.Log($"💀 La planta {plant.plantName} se ha quemado completamente.");

        // Verificar si aún hay plantas quemándose
        GameController.Instance.playerState.isAnyPlantBurning = System.Array.Exists(plants, p => p.isBurning);
        // Actualizar burnedPlot si al menos una planta está quemada
        GameController.Instance.playerState.burnedPlot = System.Array.Exists(plants, p => p.isBurned);
    }
}
