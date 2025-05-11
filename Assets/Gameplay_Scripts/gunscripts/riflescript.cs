using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class riflescript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    
    public GameObject muzzleFlash;
    private TimeManager timeManager;
    public float fireRate = 0.2f;
    private float nextTimeToFire = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
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
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        //Quaternion correctedRotation = firePoint.rotation * Quaternion.Euler(0f, 0f, 0f);
        //GameObject flash = Instantiate(mussleFlash, firePoint.position, correctedRotation);

        //GameObject flash = Instantiate(muzzleFlash, firePoint.position, muzzleFlash.transform.rotation);

        //GameObject flash = Instantiate(mussleFlash, firePoint.position, Quaternion.LookRotation(firePoint.forward, firePoint.up));
        StartCoroutine(MuzzleFlashRoutine());

        //Destroy(flash, 0.1f);
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f); // Flash duration (super quick)
        muzzleFlash.SetActive(false);
    }
}
