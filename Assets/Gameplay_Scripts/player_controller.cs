using UnityEngine;
using UnityEngine.InputSystem;

public class player_controller : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 0;

    public Camera mainCamera;
    public Camera firstPersonCamera;

    private bool isFirstPerson = true;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent <Rigidbody>();
        SetCameraView(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame) // press 'C' to toggle view
        {
            isFirstPerson = !isFirstPerson;
            SetCameraView(isFirstPerson);
        }
    }

     void OnMove (InputValue movementValue)
    {    
        
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x; 
        movementY = movementVector.y;
    }

    private void FixedUpdate() 
   {
        Vector3 move = transform.right * movementX + transform.forward * movementY;
        rb.AddForce(move * speed);
        
   }
   private void SetCameraView(bool firstPerson)
    {
        mainCamera.enabled = !firstPerson;
        firstPersonCamera.enabled = firstPerson;
    }
}
