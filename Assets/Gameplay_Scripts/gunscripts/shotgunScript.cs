using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class shotgunScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject muzzleFlash;

    public float fireRate = 1f; // Shotgun fires slower
    private float nextTimeToFire = 0f;

    public int pelletCount = 10; // Number of pellets per shot
    public float spreadAngle = 5f;

    public AudioClip shootClip;   // Shooting sound
    public AudioClip reloadClip;  // Reloading sound
    private AudioSource audioSource;
    public AmmoUI ammoUI;
    public float totalAmmo = 30f;
    public float clipAmount = 5f;
    public float clipSize = 5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Don't fire if cursor is unlocked or game is paused
        if (Cursor.lockState != CursorLockMode.Locked || Time.timeScale == 0f)
            return;

        if (Mouse.current.leftButton.isPressed && Time.unscaledTime >= nextTimeToFire)
        {
            if (clipAmount > 0)
            {
                nextTimeToFire = Time.unscaledTime + fireRate;
                clipAmount -= 1;
                Shoot();
                ammoUI.UpdateAmmo((int)clipAmount, (int)totalAmmo);
            }
            else
            {
                Debug.Log("Out of ammo in clip. Press R to reload.");
                FindObjectOfType<TutorialManager>().OnPlayerOutOfAmmo();
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Reload();
        }
    }

    public (int current, int total) GetAmmo()
    {
        return ((int)clipAmount, (int)totalAmmo);
    }

    void Shoot()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 randomDirection = Quaternion.AngleAxis(
                Random.Range(0f, spreadAngle),
                Random.insideUnitSphere) * firePoint.forward;

            Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(randomDirection));
        }

        if (shootClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootClip);
        }

        muzzleFlash.SetActive(true);
        Invoke(nameof(HideMuzzleFlash), 0.05f);
    }

    void HideMuzzleFlash()
    {
        muzzleFlash.SetActive(false);
    }

    void Reload()
    {
        if (clipAmount >= clipSize || totalAmmo <= 0) return;

        float neededAmmo = clipSize - clipAmount;

        if (totalAmmo >= neededAmmo)
        {
            clipAmount += neededAmmo;
            totalAmmo -= neededAmmo;
        }
        else
        {
            clipAmount += totalAmmo;
            totalAmmo = 0;
        }

        if (reloadClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadClip);
            StartCoroutine(UpdateAmmoUIAfterDelay(reloadClip.length));
        }
        else if (ammoUI != null)
        {
            ammoUI.UpdateAmmo((int)clipAmount, (int)totalAmmo);
        }

        Debug.Log("Reloaded. Clip: " + clipAmount + ", Total Ammo: " + totalAmmo);
        TutorialManager.Instance?.OnPlayerOutOfAmmo();
    }
    
    private IEnumerator UpdateAmmoUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ammoUI != null)
            ammoUI.UpdateAmmo((int)clipAmount, (int)totalAmmo);
    }
}
