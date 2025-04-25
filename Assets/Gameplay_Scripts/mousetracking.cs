using UnityEngine;
using UnityEngine.InputSystem;
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody; // This should be your player object

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
{
    if (Mouse.current == null) return;

    float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.unscaledDeltaTime;
    float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.unscaledDeltaTime;

    xRotation -= mouseY;
    xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    
    transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    playerBody.Rotate(Vector3.up * mouseX);
}
}