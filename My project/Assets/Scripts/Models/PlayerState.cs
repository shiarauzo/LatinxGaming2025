using Unity.VisualScripting;

public class PlayerState
{
    public bool hasCollectedSeeds = false;
    public bool burnedPlot = false;
    public bool restoredPlot = false;
    public bool isAnyPlantBurning = false;
    public bool hasExtinguishedFire = false;

    // Solo para Nivel 1;
    public PlantState[][] plantSpecies; // 3 especies, cada una con 9 parcelas;

    public PlantState GetBurningPlant()
    {
        for (int i = 0; i < plantSpecies.Length; i++)
        {
            for (int j = 0; j < plantSpecies[i].Length; j++)
            {
                if (plantSpecies[i][j].isBurning)
                    return plantSpecies[i][j];
            }
        }

        return null;
    }

    public PlantState GetBurnedPlant()
    {
        for (int i = 0; i < plantSpecies.Length; i++)
        {
            for (int j = 0; j < plantSpecies[i].Length; j++)
            {
                if (plantSpecies[i][j].isBurned)
                    return plantSpecies[i][j];
            }
        }

        return null;
    }
}