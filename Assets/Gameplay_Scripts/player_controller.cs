using UnityEngine;
using UnityEngine.InputSystem;

public class player_controller : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 10f;
    public float rotationSpeed = 700f;

    public GameObject winTextObject;

    public Camera mainCamera;
    public Camera firstPersonCamera;
    public TimeManager timeManager;
    private bool isFirstPerson = true;

    public HealthBar healthBar;
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        SetCameraView(true); // default to first person
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame) // press 'C' to toggle view
        {
            isFirstPerson = !isFirstPerson;
            SetCameraView(isFirstPerson);
        }
        if(Input.GetKeyDown("q"))
        {
            timeManager.DoSlowMotion();
        }
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        Vector3 move = transform.right * movementX + transform.forward * movementY;

        float actualSpeed = speed / Time.timeScale;
        rb.linearVelocity = new Vector3(move.x * actualSpeed, rb.linearVelocity.y, move.z * actualSpeed);
        // Optional: rotate the player to match movement direction (if you want to rotate smoothly)
       /* if (move.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }*/
    }

    private void SetCameraView(bool firstPerson)
    {
        mainCamera.enabled = !firstPerson;
        firstPersonCamera.enabled = firstPerson;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);

        Debug.Log("Player health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
        }
    }
}
