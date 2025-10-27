using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public InventoryItem itemData;
    public string objectiveName;
    private bool isPlayerInRange = false;
    public AudioSource pickupSource;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public void Interact()
    {
        InventoryManager.Instance.AddItem(itemData);
        ObjectiveManager.Instance.CompleteObjective(objectiveName);
        if (pickupSource != null)
        {
            pickupSource.Play();
        }
        Destroy(gameObject);
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