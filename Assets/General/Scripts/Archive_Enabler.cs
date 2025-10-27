using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Archive_Enabler : MonoBehaviour
{
    private bool isPlayerInRange;
    public GameObject archive;
    public string objectiveID;
    public InputActionAsset InputActions;
    public AudioSource pickUpSource;
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
        ObjectiveManager.Instance.CompleteObjective(objectiveID);
        InputActions.FindAction("Player/Archive").Enable();
        pickUpSource.Play();
        archive.SetActive(false);
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
