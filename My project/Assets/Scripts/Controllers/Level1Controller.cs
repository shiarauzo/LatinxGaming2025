using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class PlantSprite
{
    public GameObject[] parcels;
    public bool isBurning = false;
    public bool isBurned = false;
}

public class Level1Controller : MonoBehaviour
{
    [Header("Scene References")]
    public Transform plantsParent;
    public PlantSprite[] plants;
    public Level1AudioManager audioManager;

    private Dictionary<PlantState, GameObject> plantToGameObjectMap = new Dictionary<PlantState, GameObject>();
    private int currentBurningSpecies = -1;
    private float minTimeBetweenPlots = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"🌿 Level1Controller iniciado. at {Time.time}");
        // Mapear PlantState a GameObject
        LinkPlanstStatesToGameObjects();
        // Empezar el ciclo de incendios
        StartCoroutine(StartRandomFire());
    }

    private void LinkPlanstStatesToGameObjects()
    {
        var ps = GameController.Instance.playerState;

        // Mismatch
        if (ps.plantSpecies.Length != plants.Length)
            return;

        for (int speciesIndex = 0; speciesIndex < ps.plantSpecies.Length; speciesIndex++)
        {
            if (speciesIndex >= plants.Length) break;
            var specieStates = ps.plantSpecies[speciesIndex];
            var speciesObjects = plants[speciesIndex].parcels;

            int count = Mathf.Min(specieStates.Length, speciesObjects.Length);
            for (int i = 0; i < count; i++)
            {
                plantToGameObjectMap[specieStates[i]] = speciesObjects[i];
            }
        }

        Debug.Log($"🌿 Vinculadas {plantToGameObjectMap.Count} parcelas con sus GameObjects.");
    }
    private IEnumerator StartRandomFire()
    {
        // Primera especie después de 15–30s
        yield return new WaitForSeconds(Random.Range(15f, 30f));
        TryStartBurnRandomSpecies();

        // Siguientes especies cada 60-90s
        while (true)
        {
            float waitTime = Random.Range(60f, 90f);
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

    /* ********* SpreadFire ********* */
    /* Solo inicia la animación y espera el minTimeBetweenPlots antes de lanzar la siguiente parcela. */
    /* TODO:  bool fireStopped = false; */
    private IEnumerator SpreadFire(int speciesIndex)
    {
        var ps = GameController.Instance.playerState;
        var specieInDanger = ps.plantSpecies[speciesIndex];

        for (int i = 0; i < specieInDanger.Length; i++)
        {
            var parcel = specieInDanger[i];

            if (parcel.isRestored || parcel.isBurned)
                continue;

            // Si ya se apagó el fuego, se detiene la propagación
            //   if (fireStopped) break;

            if (currentBurningSpecies != speciesIndex)
            {
                Debug.Log("Terminar propagacion.");
                yield break;
            }

            // Encender la parcela
            parcel.isBurning = true;
            Debug.Log($"🔥 {parcel.GetName()} — Parcela {i + 1}/{specieInDanger.Length} en fuego (especie {speciesIndex})");

            // Actualizar el estado visual: incendiandose
            if (plantToGameObjectMap.TryGetValue(parcel, out GameObject parcelGO))
            {
                var fire = parcelGO.transform.Find("FireSprite")?.GetComponent<FireController>();
                fire?.Ignite();
                StartCoroutine(HandleBurnedParcel(parcel, fire));
            }

            // Actualizar audio 
            audioManager.UpdateIntensity(CountBurningParcels(speciesIndex));

            // Esperar al menos el minTimeBetweenPlots antes de iniciar la siguiente parcela
            yield return StartCoroutine(WaitBeforeNextParcel(parcel, speciesIndex));
        }
    }

    private IEnumerator HandleBurnedParcel(PlantState parcel, FireController fire)
    {
        yield return new WaitForSeconds(minTimeBetweenPlots + 5f);

        // Si sigue quemándose, se quema
        if (parcel.isBurning)
        {
            parcel.isBurning = false;

            // Actualizar estado visual: quemada
            if (fire != null)
            {
                // Lanza la animación de extinción en paralelo
                fire.Extinguish(true, () =>
                {
                    parcel.isBurned = true;
                    Debug.Log($"DESDE LEVEL1 CONTROLLER 💀 {parcel.GetName()} se ha quemado (finalizado).");
                    CheckSpeciesCompletion(parcel);
                });
            }
            else
            {
                parcel.isBurned = true;
                CheckSpeciesCompletion(parcel);
            }

            // Actualizar audio
            audioManager.UpdateIntensity(CountBurningParcelsForParcel(parcel));
        }
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

    private int CountBurningParcelsForParcel(PlantState parcel)
    {
        int speciesIndex = FindSpeciesIndex(parcel);
        return CountBurningParcels(speciesIndex);
    }
    
    private int FindSpeciesIndex(PlantState parcel)
    {
        var ps = GameController.Instance.playerState.plantSpecies;
        for (int i = 0; i < ps.Length; i++)
            if (System.Array.Exists(ps[i], p => p == parcel))
                return i;
        return -1;
    }
    private IEnumerator WaitBeforeNextParcel(PlantState parcel, int speciesIndex)
    {
        float timer = 0f;
        while (timer < minTimeBetweenPlots)
        {
            if (!parcel.isBurning)
            {
                Debug.Log($"💧 Parcela apagada a tiempo; deteniendo propagación en especie {speciesIndex}.");
                if (plantToGameObjectMap.TryGetValue(parcel, out GameObject pGO))
                {
                    var fire = pGO.transform.Find("FireSprite")?.GetComponent<FireController>();
                    fire?.Extinguish(false);
                }

                // currentBurningSpecies = -1;
                audioManager.UpdateIntensity(CountBurningParcels(speciesIndex));
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }
    
    private void CheckSpeciesCompletion(PlantState parcel)
    {
        int speciesIndex = FindSpeciesIndex(parcel);
        var specie = GameController.Instance.playerState.plantSpecies[speciesIndex];

        bool allBurned = true;
        foreach (var p in specie)
            if (!p.isBurned)
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

            if (!GameController.Instance.hasWon && !GameController.Instance.hasLost)
                audioManager.FadeOutAuxTracksExceptBackground();
        }
        else
            Debug.Log($"🌿 La especie {speciesIndex} fue salvada a tiempo.");

        audioManager.UpdateIntensity(CountBurningParcels(speciesIndex));
        CheckWinLose();
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

            GameController.Instance.hasWon = true;
            // TODO: cargar escena de victoria
            // SceneManager.LoadScene("VictoryScene");
            audioManager.PlayVictory();
        }
        else if (allBurned)
        {
            // Lógica para manejar la derrota
            Debug.Log("💀 Todas las plantas se han quemado. Has perdido.");

            GameController.Instance.hasLost = true;
            // TODO: cargar escena de LOSE
            // SceneManager.LoadScene("GameOverScene");
            audioManager.PlayLose();
        }
    }
}
