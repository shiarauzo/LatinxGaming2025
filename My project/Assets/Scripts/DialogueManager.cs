// Assets/Scripts/DialogueManager.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public float typingSpeed = 0.05f;
    [TextArea(2, 6)] public string[] lines;

    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    public void NextLine()
    {
        // If there are no more lines, end the dialogue
        if (currentLineIndex >= lines.Length)
        {
            EndDialogue();
            return;
        }

        // If it's typing, finish immediately
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = lines[currentLineIndex];
            currentLineIndex++;
            return;
        }

        typingCoroutine = StartCoroutine(TypeLine(lines[currentLineIndex]));
        currentLineIndex++;
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    private void EndDialogue()
    {
        // Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // PrincipalMap
    }
    
    public void StartDialogue()
    {
        currentLineIndex = 0;
        NextLine();
    }
}
