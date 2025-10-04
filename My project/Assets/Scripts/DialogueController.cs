// Assets/Scripts/DialogueController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string text;
    public AudioClip voice;
}

[System.Serializable]
public class DialogueData
{
    public DialogueLine[] spanishLines;
    public DialogueLine[] englishLines;
}
public class DialogueController : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public DialogueData dialogueData;
    public AudioSource voiceSource;
    public enum Language { Spanish, English }
    public Language currentLanguage = Language.English;

    private DialogueLine[] currentLines;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

         if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void Update()
    {
        // Next line on Enter key
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            NextLine();
        }    
    }

    public void NextLine()
    {
        // If there are no more lines, end the dialogue
        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        // If it's typing, finish immediately
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = currentLines[currentLineIndex].text;
            currentLineIndex++;
            return;
        }

        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLineIndex]));
        currentLineIndex++;
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        dialogueText.text = "";

        // Reproducir voz
        if (line.voice != null && voiceSource != null)
        {
            if (voiceSource.isPlaying)
                voiceSource.Stop();
            
            voiceSource.clip = line.voice;
            voiceSource.Play();
        }

        foreach (char letter in line.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    private void EndDialogue()
    {
        // Hide the dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        
        // Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartDialogue()
    {
        // Show the dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        switch (currentLanguage)
        {
            case Language.Spanish:
                currentLines = dialogueData.spanishLines;
                break;
            case Language.English:
                currentLines = dialogueData.englishLines;
                break;
        }

        currentLineIndex = 0;
        NextLine();
    }
}
