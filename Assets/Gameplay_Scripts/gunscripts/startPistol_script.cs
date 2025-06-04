using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class startPistol_script : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public GameObject muzzleFlash;
    private TimeManager timeManager;
    public float fireRate = 0.2f;
    private float nextTimeToFire = 0f;

    public AudioClip shootClip; // Shooting sound
    public AudioClip reloadClip; // Reloading sound

    private AudioSource audioSource;
    public AmmoUI ammoUI;
    public bool isReloading = false;
    public float totalAmmo = 150f;
    public float clipAmount = 25f;
    public float clipSize = 25f;

    void Start()
    {
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Mouse.current == null) return;

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
                TutorialManager.Instance?.OnPlayerOutOfAmmo();
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
        //Quaternion bulletRotation = Quaternion.LookRotation(firePoint.forward);
        //Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (shootClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootClip);
        }

        StartCoroutine(MuzzleFlashRoutine());
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
        FindObjectOfType<TutorialManager>().OnPlayerReloaded();
    }

    private IEnumerator UpdateAmmoUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ammoUI != null)
            ammoUI.UpdateAmmo((int)clipAmount, (int)totalAmmo);
    }
    private IEnumerator MuzzleFlashRoutine()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }
}
