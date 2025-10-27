using UnityEngine;

public class Archive_rotation : MonoBehaviour
{
    public float speed;
    private float oldMouseX = 0;
    private float oldMouseY = 0;
    private float xOffset;
    private float yOffset;

    // Update is called once per frame
    void FixedUpdate()
    {
        xOffset = (oldMouseX - Input.mousePosition.x) * Time.deltaTime * speed;
        yOffset = (oldMouseY - Input.mousePosition.y) * Time.deltaTime * speed;
        Debug.Log(xOffset);
        this.gameObject.transform.Rotate(new Vector3(yOffset, xOffset, 0f), Space.Self);
        oldMouseX = Input.mousePosition.x;
        oldMouseY = Input.mousePosition.y;
    }
}
