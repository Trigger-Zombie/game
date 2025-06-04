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
    public GameObject deathScreenPanel;
    public MouseLook mouseLookScript;
    public CrosshairManager crosshairManager;


    public Transform swordMount;

    public float jumpForce = 7f;
    private bool isGrounded = true;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        SetCameraView(true); // default to first person
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
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
         
        if (movementX != 0 || movementY != 0)
        {
            TutorialManager.Instance?.OnPlayerMoved();
        }

    }

    private void SetCameraView(bool firstPerson)
    {
        mainCamera.enabled = !firstPerson;
        firstPersonCamera.enabled = firstPerson;
    }

    void Die()
    {
        Debug.Log("Player has died.");

        // Show the death screen
        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(true);

        // Stop the game
        Time.timeScale = 0f;

        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player controls
        this.enabled = false; // Disables player_controller
        if (mouseLookScript != null)
            mouseLookScript.enabled = false; // Disables mouse look
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
                Die();
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

            string weaponType = gunSlots[slotIndex].tag;
            crosshairManager.ShowCrosshair(weaponType);

            // Try updating the AmmoUI immediately
            AmmoUI ammoUI = Object.FindFirstObjectByType<AmmoUI>();
            var gun = gunSlots[slotIndex];

            // Try getting the ammo info
            if (gun.TryGetComponent<riflescript>(out var rifle))
            {
                var (cur, total) = rifle.GetAmmo();
                ammoUI.UpdateAmmo(cur, total);
            }
            else if (gun.TryGetComponent<shotgunScript>(out var shotgun))
            {
                var (cur, total) = shotgun.GetAmmo();
                ammoUI.UpdateAmmo(cur, total);
            }
            else if (gun.TryGetComponent<startPistol_script>(out var pistol))
            {
                var (cur, total) = pistol.GetAmmo();
                ammoUI.UpdateAmmo(cur, total);
            }
        }
    }

    public void PickupGun(GameObject newGun)
    { // use playerControllerRef.PickupGun(gunPrefab); from a source to let the gun pickup

        bool isMelee = newGun.CompareTag("sword");

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

        Transform mount = isMelee ? swordMount : weaponMount;

        // Instantiate and parent weapon
        //GameObject newGunInstance = Instantiate(newGun, mount);
        //Debug.Log("Spawned melee weapon: " + newGunInstance.name);
        //GameObject newGunInstance = Instantiate(newGun, weaponMount);
        if (isMelee)
        {
            GameObject newWeaponInstance = Instantiate(newGun);

            newWeaponInstance.name = "sword";
            newWeaponInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
            // 🔧 Position it using sword_mount
            newWeaponInstance.transform.position = swordMount.position;
            newWeaponInstance.transform.rotation = swordMount.rotation;

            // 🔧 Parent it directly to sword_holder
            newWeaponInstance.transform.SetParent(swordMount.parent); // sword_holder

            newWeaponInstance.transform.localScale = Vector3.one; // just to be safe
            newWeaponInstance.SetActive(false);

            gunSlots[targetSlot] = newWeaponInstance;
            Equip(targetSlot);

            // 🔧 Hook up the hitbox
            SwordAttack swordAttack = swordMount.parent.GetComponent<SwordAttack>();
            if (swordAttack != null)
            {
                SwordHitbox hitbox = newWeaponInstance.GetComponentInChildren<SwordHitbox>();
                if (hitbox != null)
                {
                    swordAttack.hitbox = hitbox;
                    Debug.Log("Hooked up SwordHitbox to SwordAttack");
                }
                else
                {
                    Debug.LogWarning("No SwordHitbox found on sword prefab");
                }
            }
        }
        else
        {
            Debug.Log("PickupGun called with: " + newGun.name + ", tag: " + newGun.tag);

            GameObject newGunInstance = Instantiate(newGun, mount);
            newGunInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
            newGunInstance.transform.localPosition = Vector3.zero;
            newGunInstance.transform.localRotation = Quaternion.identity;
            if (newGunInstance.CompareTag("Shotgun"))
            {

                newGunInstance.transform.localRotation = Quaternion.Euler(0f, -90f, 0f); // or whatever fixes it
            }
            newGunInstance.SetActive(false); // Don't auto-fire unless equipped

            gunSlots[targetSlot] = newGunInstance;
            Equip(targetSlot); // Optional: auto-equip the new gun

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
