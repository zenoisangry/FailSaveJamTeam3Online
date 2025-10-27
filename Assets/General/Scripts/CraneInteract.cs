using UnityEngine;

public class CraneInteract : MonoBehaviour
{

    public LockedDoor requiredDoor;
    private bool isPlayerInRange;
    public GameObject crate;
    public GameObject archive;
    public string objectiveID;
    public AudioSource source;
    private bool used = false;
    // Update is called once per frame
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && requiredDoor.open && !used)
        {
            Interact();
        }
    }

    public void Interact()
    {
        source.Play();
        crate.SetActive(false);
        archive.SetActive(true);
        ObjectiveManager.Instance.CompleteObjective(objectiveID);
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
