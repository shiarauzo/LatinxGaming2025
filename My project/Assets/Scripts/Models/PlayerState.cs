    public class PlayerState
    {
        public bool hasCollectedSeeds = false;
        public bool burnedPlot = false;
        public bool restoredPlot = false;
        public bool isAnyPlantBurning = false;

        public bool hasExtinguishedFire = false;

        // Solo para Nivel 1;
        public PlantState[][] plantSpecies; // 3 especies, cada una con 9 parcelas;

    }