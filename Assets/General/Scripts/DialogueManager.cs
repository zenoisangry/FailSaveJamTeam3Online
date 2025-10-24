using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.2f;

    public Animator animator;

    public AudioSource dialogueSounds;
    private float audioCD = 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;

        //animator.Play("show");

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

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
        //dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            //dialogueArea.text += letter;
            audioCD--;
            if (letter == (char)32) audioCD += 1;
            if (audioCD <= 0)
            {
                dialogueSounds.pitch = Random.Range(0.90f, 1.1f);
                dialogueSounds.volume = 1;
                dialogueSounds.Play();
                audioCD = 1;
                StartCoroutine(FadeOut());
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        audioCD = 0;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        //animator.Play("hide");
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(typingSpeed / 3);
        float timer = typingSpeed/2;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            dialogueSounds.volume -= Time.deltaTime / (typingSpeed/2);
            yield return null;
        }
    }
}