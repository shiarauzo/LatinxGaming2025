using UnityEngine;

[CreateAssetMenu(fileName="NewNPCDialogue", menuName ="NPC Dialogue")]

// Object that can hold diff types of data
public class NPCDialogue : ScriptableObject
{
    public string npcNameKey;
    public Sprite npcPortrait;

    [System.Serializable]
    public class DialogueState
    {
        public string stateName;
        public string[] englishLines;
        public string[] spanishLines;
        public bool[] autoProgressLines;
        public float autoProgressDelay = 1.5f; //Start speeking the next one
        public float typingSpeed = 0.05f;
        public AudioClip voiceSound;
    }

    public DialogueState[] dialogueStates;
}
