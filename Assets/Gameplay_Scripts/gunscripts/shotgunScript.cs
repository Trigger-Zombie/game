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
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public AudioClip shootClip; // Audio file to play
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.isPressed && Time.unscaledTime >= nextTimeToFire)
        {
            nextTimeToFire = Time.unscaledTime + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {   //Debug.DrawRay(firePoint.position, firePoint.forward * 2f, Color.red, 2f);

        for (int i = 0; i < pelletCount; i++)
    {   /*
        // Calculate random spread
        float angleY = Random.Range(-spreadAngle, spreadAngle);
        float angleX = Random.Range(-spreadAngle, spreadAngle);
        // Create the spread direction
        Vector3 spreadDirection = firePoint.forward;  // Use the forward direction of the firePoint
        spreadDirection = Quaternion.Euler(angleX, angleY, 0f) * spreadDirection;  // Apply the spread rotation to the direction
        // Instantiate the bullet at the firePoint position and give it the spread direction
        GameObject pellet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(spreadDirection));
        */
        Vector3 randomDirection = Quaternion.AngleAxis(
        Random.Range(0f, spreadAngle), 
        Random.insideUnitSphere) * firePoint.forward;

        GameObject pellet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(randomDirection));
        
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
}
