using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    public bool isTruck;
    public LockedDoor requiredDoor;
    public string requiredItemId;
    public DoorMovement door;
    public string nextObjectiveId;
    public bool open;
    private bool hasMoved = false;
    private bool isPlayerInRange;
    public DialogueTrigger failDialogue;

    public void Interact()
    {
            if (InventoryManager.Instance.HasItem(requiredItemId))
            {
                if (!isTruck)
                {
                    if (!open)
                    {
                        door.OpenDoor();
                        open = true;
                    }
                    else
                    {
                        door.CloseDoor();
                        open = false;
                    }
                }
                else
                {
                bool available = true;
                    if (requiredDoor != null)
                    {
                    if (!requiredDoor.open)
                    {
                        available = false;
                    }
                    else
                        available = true;
                    }
                    if (!open && available)
                    {
                        ObjectiveManager.Instance.CompleteObjective(nextObjectiveId);
                        door.OpenDoor();
                        open = true;
                        hasMoved = true;
                    }
                    else
                    {
                        if (failDialogue != null)
                        {
                            if (!hasMoved) failDialogue.TriggerDialogue();
                        }
                    }
                }
            }
            else
            {
            if (failDialogue != null)
                {
                    if (!hasMoved) failDialogue.TriggerDialogue();
                }
            }
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