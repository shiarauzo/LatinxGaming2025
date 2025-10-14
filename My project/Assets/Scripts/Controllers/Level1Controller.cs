using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlantSprite
{
    public GameObject[] parcels;
    public bool isBurning = false;
    public bool isBurned = false;
}

public class Level1Controller : MonoBehaviour
{
    public PlantSprite[] plants;
    public Level1AudioManager audioManager;
    private int currentBurningSpecies = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartRandomFire());
    }

    private IEnumerator StartRandomFire()
    {
        // Primera especie después de 15–30s
        yield return new WaitForSeconds(Random.Range(15f, 30f));
        TryStartBurnRandomSpecies();

        // Siguientes especies cada 60-90s
        while (true)
        {
            float waitTime = Random.Range(10f, 20f);//60f, 90f
            yield return new WaitForSeconds(waitTime);

            TryStartBurnRandomSpecies();
        }
    }

    private void TryStartBurnRandomSpecies()
    {
        if (currentBurningSpecies != -1)
        {
            Debug.Log("🔥 Ya hay una especie quemándose. Esperando a que termine.");
            return;
        }

        // Elegir especie válida
        var ps = GameController.Instance.playerState;
        var available = new List<int>();
        for (int i = 0; i < ps.plantSpecies.Length; i++)
        {
            var specieInDanger = ps.plantSpecies[i];
            bool allBurned = true;
            bool allRestored = true;
            foreach (var parcel in specieInDanger)
            {
                if (!parcel.isBurned) allBurned = false;
                if (!parcel.isRestored) allRestored = false;
            }
            if (!allBurned && !allRestored) available.Add(i);
        }

        if (available.Count == 0) return;

        int speciesIndex = available[Random.Range(0, available.Count)];

        currentBurningSpecies = speciesIndex;
        StartCoroutine(BurnRandomSpecies(speciesIndex));
    }
    
    private IEnumerator BurnRandomSpecies(int speciesIndex)
    {
        var ps = GameController.Instance.playerState;
        Debug.Log($"🔥 Empezando a quemar la especie {speciesIndex} ({ps.plantSpecies[speciesIndex][0].GetName()})");

        // Lanza la propagación y espera que termine (blocking)
        yield return StartCoroutine(SpreadFire(speciesIndex));

        // Resetea flag cuando termine todo el proceso de esa especie
        currentBurningSpecies = -1;
    }
    
    private IEnumerator SpreadFire(int speciesIndex)
    {
        var ps = GameController.Instance.playerState;
        var specieInDanger = ps.plantSpecies[speciesIndex];
     //   bool fireStopped = false;

        for (int i = 0; i < specieInDanger.Length; i++)
        {
            var parcel = specieInDanger[i];

            // Si ya fue restaurada o quemada, saltar
            if (parcel.isRestored || parcel.isBurned)
                continue;

            // Si ya se apagó el fuego, se detiene la propagación
         //   if (fireStopped) break;

            // Si otra corutina cambió currentBurningSpecies, terminar
            if (currentBurningSpecies != speciesIndex)
            {
                Debug.Log("Propagation aborted: species changed.");
                yield break;
            }

            // Encender la parcela
            parcel.isBurning = true;
            Debug.Log($"🔥 {parcel.GetName()} — Parcela {i+1}/{specieInDanger.Length} en fuego (especie {speciesIndex})");

            // Actualizar audio contando SOLO esta especie
            audioManager.UpdateIntensity(CountBurningParcels(speciesIndex));
             
            // Ventana para que el jugador apague (si se apaga, parcel.isBurning = false, parcel.isRestored = true)
            // Esperar 10s para dar chance de apagar (antes de pasar a quemada)
            float burnDuration = 10f;
            float elapsed = 0f;

            while (elapsed < burnDuration)
            {
                yield return new WaitForSeconds(1f);
                elapsed += 1f;

                // Si el jugador la apagó
                if (!parcel.isBurning)
                {
                    Debug.Log($"💧 Parcela {i + 1} apagada a tiempo; deteniendo propagación en especie {speciesIndex}.");
                    // liberar flag y salir
                    currentBurningSpecies = -1;
                    // actualizar audio
                    audioManager.UpdateIntensity(CountBurningParcels(speciesIndex));
                    yield break;
                }
            }

            // Si no se apagó el incendio, se quema
            if (parcel.isBurning)
            {
                parcel.isBurning = false;
                parcel.isBurned = true;
                Debug.Log($"💀 Parcela {i + 1} de especie {speciesIndex} se ha quemado.");
            }
            
            // actualizar audio y esperar un poco antes de la siguiente parcela
            audioManager.UpdateIntensity(CountBurningParcels(speciesIndex));
            yield return new WaitForSeconds(Random.Range(0f, 2f));
        }

        // Si todas se quemaron
        bool allBurned = true;
        foreach (var parcel in specieInDanger)
            if (!parcel.isBurned)
            {
                allBurned = false;
                break;
            }

        if (allBurned)
        {
            Debug.Log($"💀 La especie {speciesIndex} se quemó completamente.");
            if (plants != null && speciesIndex < plants.Length)
            {
                plants[speciesIndex].isBurned = true;
                plants[speciesIndex].isBurning = false;
            }
        }
        else
            Debug.Log($"🌿 La especie {speciesIndex} fue salvada a tiempo.");
        audioManager.UpdateIntensity(CountBurningParcels(speciesIndex));
        CheckWinLose();
    }
    
    private int CountBurningParcels(int speciesIndex)
    {
        int burningCount = 0;
        var specie = GameController.Instance.playerState.plantSpecies[speciesIndex];

        foreach (var parcel in specie)
            if (parcel.isBurning || parcel.isBurned)
                burningCount++;
        
        return burningCount;
    }
    public void CheckWinLose()
    {
        var ps = GameController.Instance.playerState;

        bool allSaved = true;
        bool allBurned = true;

        foreach (var species in ps.plantSpecies)
        {
            foreach (var parcel in species)
            {
                if (!parcel.isRestored) allSaved = false;
                if (!parcel.isBurned) allBurned = false;
            }
        }

        if (allSaved)
        {
            // Lógica para manejar la victoria
            Debug.Log("🎉 ¡Has salvado todas las plantas! ¡Ganaste!");
            
            // TODO: cargar escena de victoria
            // SceneManager.LoadScene("VictoryScene");
            audioManager.PlayVictory();
        }
        else if (allBurned)
        {
            // Lógica para manejar la derrota
            Debug.Log("💀 Todas las plantas se han quemado. Has perdido.");
            
            // TODO: cargar escena de LOSE
            // SceneManager.LoadScene("GameOverScene");
            audioManager.PlayLose();
        }
    }
}
