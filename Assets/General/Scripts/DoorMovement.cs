using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorMovement : MonoBehaviour
{
    public float yoffsetOpen;
    public float openingTime;
    public bool isTruck;
    public AudioSource movementAudio;

    public void OpenDoor()
    {
        if (movementAudio != null)
        {
            movementAudio.Play();
        }
        StartCoroutine(SmoothMovement(yoffsetOpen));
    }

    public void CloseDoor()
    {
        if (movementAudio != null)
        {
            movementAudio.Play();
        }
        StartCoroutine(SmoothMovement(-yoffsetOpen));
    }

    IEnumerator SmoothMovement(float targetY)
    {
        Debug.Log("moving...");
        float currentY = transform.position.y;
        float t = 0;
        while (t < openingTime)
        {
            t += Time.deltaTime;
            if (!isTruck)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + targetY * Time.deltaTime, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x + targetY * Time.deltaTime, transform.position.y, transform.position.z);
            }
            
            yield return null;
        }
    }
}
