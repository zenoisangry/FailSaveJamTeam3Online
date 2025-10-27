using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public Canvas dialogueCanvas;
    public TextMeshProUGUI dialogueArea;

    [Header("References")]
    public PlayerMovement playerMovement;
    public InputActionAsset inputActions;

    [Header("Settings")]
    public float typingSpeed = 0.2f;
    public AudioSource dialogueSounds;

    private Queue<DialogueLine> lines;
    private float audioCD = 1f;
    public bool isDialogueActive = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        lines = new Queue<DialogueLine>();

        if (dialogueCanvas == null)
        {
            Debug.LogError("Dialogue Canvas non assegnata nel DialogueManager!");
        }
        else
        {
            dialogueCanvas.gameObject.SetActive(false);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        inputActions.FindActionMap("Player").Disable();
        inputActions.FindActionMap("UI").Enable();

        if (dialogueCanvas == null)
        {
            Debug.LogError("Impossibile avviare il dialogo: Canvas non assegnata.");
            return;
        }

        isDialogueActive = true;
        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
            lines.Enqueue(dialogueLine);

        dialogueCanvas.gameObject.SetActive(true);
        Debug.Log("Canvas attivata: " + dialogueCanvas.name);

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";

        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;

            audioCD--;
            if (letter == ' ') audioCD += 1;
            if (audioCD <= 0)
            {
                dialogueSounds.pitch = Random.Range(0.90f, 1.1f);
                dialogueSounds.volume = 1;
                dialogueSounds.Play();
                audioCD = 3;
                StartCoroutine(FadeOut());
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        audioCD = 0;
    }

    void EndDialogue()
    {
        inputActions.FindActionMap("UI").Disable();
        inputActions.FindActionMap("Player").Enable();

        isDialogueActive = false;

        if (dialogueCanvas != null)
        {
            dialogueCanvas.gameObject.SetActive(false);
            Debug.Log("Canvas disattivata: " + dialogueCanvas.name);
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(typingSpeed*2f);
        float timer = typingSpeed;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            dialogueSounds.volume -= Time.deltaTime / (typingSpeed);
            yield return null;
        }
    }
}