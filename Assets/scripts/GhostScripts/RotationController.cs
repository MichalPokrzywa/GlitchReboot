using UnityEngine;

public class RotationController : MonoBehaviour
{
    InputInterface input = new StandardInput();
    public float sensX;
    public float sensY;

    public float xRotation;
    public float yRotation;

    void Start()
    {
        var Input = gameObject.GetComponent<InputInterface>();
        if (Input != null)
            input = Input;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        float mouseX = input.GetMouseX() * Time.deltaTime * sensX;
        float mouseY = input.GetMouseY() * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
