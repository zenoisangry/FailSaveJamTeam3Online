using UnityEngine;

public class MissionTrigger : MonoBehaviour, IInteractable
{
    public string nextObjectiveId;

    public void Interact()
    {
        ObjectiveManager.Instance.SetObjective(nextObjectiveId);
    }
}