using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public InventoryItem itemData;

    public void Interact()
    {
        InventoryManager.Instance.AddItem(itemData);
        ObjectiveManager.Instance.CompleteObjective("ottieni_chiave");
        Destroy(gameObject);
    }
}