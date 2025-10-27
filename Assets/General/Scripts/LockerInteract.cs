using UnityEngine;

public class LockerInteract : MonoBehaviour
{
    public bool isRight;
    public DialogueTrigger lockerDialogue;
    private bool isPlayerInRange = false;
    public InventoryItem itemData;
    public AudioSource pickUpSource;
    private bool opened = false;

    public void Interact()
    {
        lockerDialogue.TriggerDialogue();
        if (isRight)
        {
            InventoryManager.Instance.AddItem(itemData);
        }
        if (pickUpSource != null) {
            if (!opened)
            {
                pickUpSource.Play();
            }
        }
        opened = true;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
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
