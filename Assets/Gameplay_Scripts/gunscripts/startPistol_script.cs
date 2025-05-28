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
            }
            else
            {
                Debug.Log("Out of ammo in clip. Press R to reload.");
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Reload();
        }
    }

    void Shoot()
    {
        Quaternion bulletRotation = Quaternion.LookRotation(firePoint.forward);
        Instantiate(bulletPrefab, firePoint.position, bulletRotation);

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
        }

        Debug.Log("Reloaded. Clip: " + clipAmount + ", Total Ammo: " + totalAmmo);
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }
}
