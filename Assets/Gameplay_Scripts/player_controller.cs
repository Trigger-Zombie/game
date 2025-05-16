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
    public GameObject[] gunSlots = new GameObject[2]; // Slot 0 = Key 1, Slot 1 = Key 2
    private int currentGunIndex = 0;

    public Transform weaponMount;

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
        if (Input.GetKeyDown("q"))
        {
            timeManager.DoSlowMotion();
        }
        if (Keyboard.current.digit1Key.wasPressedThisFrame) Equip(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) Equip(1);
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

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // Clamp between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthBar.SetHealth(currentHealth);

        if (amount > 0)
        {
            //Debug.Log("Took damage: " + amount);
            if (currentHealth <= 0)
            {
                Debug.Log("Player has died.");
                // Add death logic here
            }
        }
        else if (amount < 0)
        {
            Debug.Log("Healed: " + Mathf.Abs(amount));
            // Optional: add healing effects here
        }
    }

    void Equip(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= gunSlots.Length) return;

        // Disable all guns first
        foreach (var gun in gunSlots)
        {
            if (gun != null) gun.SetActive(false);
        }

        // Enable the selected one
        if (gunSlots[slotIndex] != null)
        {
            gunSlots[slotIndex].SetActive(true);
            currentGunIndex = slotIndex;
        }
    }
    
    public void PickupGun(GameObject newGun){ // use playerControllerRef.PickupGun(gunPrefab); from a source to let the gun pickup
        // Find the first empty slot, or overwrite the currently equipped one
        int targetSlot = -1;
        for (int i = 0; i < gunSlots.Length; i++)
        {
            if (gunSlots[i] == null)
            {
                targetSlot = i;
                break;
            }
        }

        // If all slots are full, overwrite the currently equipped one
        if (targetSlot == -1)
        {
            Destroy(gunSlots[currentGunIndex]);
            targetSlot = currentGunIndex;
        }

        // Instantiate the new gun and parent it to the player (e.g., hand or weapon mount)
        //GameObject newGunInstance = Instantiate(newGun, transform);
        //GameObject newGunInstance = Instantiate(newGun, weaponMount.position, weaponMount.rotation, weaponMount);
        GameObject newGunInstance = Instantiate(newGun, weaponMount);
        newGunInstance.transform.localPosition = Vector3.zero;
        newGunInstance.transform.localRotation = Quaternion.identity;
        if (newGunInstance.CompareTag("Shotgun")){

            newGunInstance.transform.localRotation = Quaternion.Euler(0f, -90f, 0f); // or whatever fixes it
        }
        newGunInstance.SetActive(false); // Don't auto-fire unless equipped

        gunSlots[targetSlot] = newGunInstance;
        Equip(targetSlot); // Optional: auto-equip the new gun
    }

}
