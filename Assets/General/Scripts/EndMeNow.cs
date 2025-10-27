using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndMeNow : MonoBehaviour
{
    private bool isPlayerInRange;
    public string requiredItemId;
    public DialogueTrigger failDialogue;
    public SpriteRenderer fadeToBlack;
    public float FadeToBlackDuration;
    public AudioSource endSceneAudio;
    // Update is called once per frame
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (InventoryManager.Instance.HasItem(requiredItemId))
        {
            ObjectiveManager.Instance.CompleteObjective("obj_04");
        }
        else
        {
            failDialogue.TriggerDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Premi E per parlare");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Fuori portata dell'NPC");
        }
    }
}
