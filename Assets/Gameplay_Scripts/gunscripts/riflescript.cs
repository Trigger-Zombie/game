using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Xml.Schema;
using UnityEngine.UI;

public class riflescript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public GameObject muzzleFlash;
    private TimeManager timeManager;
    public float fireRate = 0.2f;
    private float nextTimeToFire = 0f;

    public AudioClip shootClip; // Audio file to play
    public AudioClip reloadClip; // Reload sound to play

    private AudioSource audioSource;
    public AmmoUI ammoUI;
    public float totalAmmo = 150f;
    public float clipAmount = 25f;
    public float clipSize = 25f;

    void Start()
    {
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        audioSource = GetComponent<AudioSource>();
        if (ammoUI == null)
        {
            GameObject go = GameObject.Find("AmmoText");
            if (go != null)
            {
                ammoUI = go.GetComponent<AmmoUI>();
                if (ammoUI == null)
                {
                    Debug.LogError("Found GameObject 'AmmoText' but no AmmoUI component on it!");
                }
            }
            else
            {
                Debug.LogError("Could not find a GameObject named 'AmmoText' in the scene.");
            }
        }
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
        TutorialManager.Instance?.OnPlayerOutOfAmmo();
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
