using UnityEngine;
using UnityEngine.InputSystem;
public class MouseLook : MonoBehaviour
{
    
    public float mouseSensitivity = 50f;
    public Transform playerBody; //player object

    float xRotation = 0f;
    private float yRotation = 0f; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Get mouse movement
        float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.unscaledDeltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.unscaledDeltaTime;

        // Update camera's x-rotation (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        // Apply rotation to camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Update the player’s body rotation (yaw)
        yRotation += mouseX;
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f); // Rotate only along the y-axis (yaw)
    }
}