using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentalNPC : BaseNPC
{
    [Header("Planta asociada")]
    public int speciesIndex; // 0: Orchids, 1: CatsClaw, 2: Cacao
    protected override NPCDialogue.DialogueState GetCurrentDialogueState()
    {
        var states = dialogueData.dialogueStates;
        var player = GameController.Instance.playerState;
        var NPCSpecie = player.plantSpecies[speciesIndex];

        bool anyBurning = false;
        bool anyBurned = false;

        foreach (var parcel in NPCSpecie)
        {
            if (parcel.isBurning)
                anyBurning = true;
            else if (parcel.isBurned)
                anyBurned = true;
        }

        if (anyBurning)
            return System.Array.Find(states, s => s.stateName == "isBurning");
        if (anyBurned)
            return System.Array.Find(states, s => s.stateName == "isBurned");

        return System.Array.Find(states, s => s.stateName == "isRestored");
    }
}

