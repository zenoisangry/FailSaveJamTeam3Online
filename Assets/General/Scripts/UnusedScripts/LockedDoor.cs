using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    public string requiredItemId;
    public Animator animator;
    public string nextObjectiveId;

    public void Interact()
    {
        if (InventoryManager.Instance.HasItem(requiredItemId))
        {
            animator.SetTrigger("Open");
            if (!string.IsNullOrEmpty(nextObjectiveId))
                ObjectiveManager.Instance.SetObjective(nextObjectiveId);
        }
        else
        {
            Debug.Log("Serve una chiave per aprire questa porta.");
        }
    }
}