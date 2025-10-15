using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ChullachaquiNPC : BaseNPC
{
    [Header("Internal States")]
    public bool firstEncounter = true;
    
    // Determina el estado de diálogo actual según PlayerState
    protected override NPCDialogue.DialogueState GetCurrentDialogueState()
    {
        var states = dialogueData.dialogueStates;
        var player = GameController.Instance.playerState;

        // Prioridad al fuego, incluso en la primera interacción
        if (player.isAnyPlantBurning)
            return System.Array.Find(states, s => s.stateName == "FireAlert");


        // First Encounter
        if (firstEncounter)
        {
            firstEncounter = false;
            return System.Array.Find(states, s => s.stateName == "FirstEncounter");
        }
            
        // Initial Reminder: no ha recolectado semillas ni apagado incendios
        if (!player.hasCollectedSeeds && !player.hasExtinguishedFire && !player.burnedPlot)
            return System.Array.Find(states, s => s.stateName == "InitialReminder");

        // Burned Plot + Seeds: terreno quemado y jugador tiene semillas
        if (player.burnedPlot && player.hasCollectedSeeds)
            return System.Array.Find(states, s => s.stateName == "BurnedPlotWithSeeds");

        // Burned Plot + No Seeds: terreno quemado y jugador no tiene semillas
        if (player.burnedPlot && !player.hasCollectedSeeds)
            return System.Array.Find(states, s => s.stateName == "BurnedPlotNoSeeds");

        // Restored Plot: terreno/planta restaurado
        if (player.restoredPlot)
            return System.Array.Find(states, s => s.stateName == "RestoredPlot");

        // Plot not yet restored: terreno quemado pero no restaurado
        if (player.burnedPlot && !player.restoredPlot)
            return System.Array.Find(states, s => s.stateName == "PlotNotYetRestored");
            
        // Partial Reminder: catch-all si ninguna otra condición se cumple
        return System.Array.Find(states, s => s.stateName == "PartialReminder");
    }
}