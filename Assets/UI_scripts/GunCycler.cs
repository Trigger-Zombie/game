using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCycler : MonoBehaviour
{
    [Header("Gun Cycling Settings")]
    public List<GameObject> gunPrefabs;    // List of gun prefabs to cycle through
    public Transform spawnPoint;           // Where the gun should appear
    public float cycleInterval = 0.2f;     // Time between gun switches
    public float totalCycleTime = 3f;      // How long to cycle before choosing final gun

    private GameObject currentGunInstance;

    public void StartCycling()
    {
        // Hide cube's visual
        Debug.Log("Hiding cube");
        Transform visualMesh = transform.Find("Cube");
        if (visualMesh != null){
            visualMesh.gameObject.SetActive(false);
        }
        StartCoroutine(CycleGuns());
    }

    IEnumerator CycleGuns()
    {
        float timer = 0f;
        int index = 0;

        while (timer < totalCycleTime)
        {
            if (currentGunInstance != null)
                Destroy(currentGunInstance);
            int tempIndex = index % gunPrefabs.Count;
            Debug.Log("tempIndex = " + tempIndex + ", prefab = " + gunPrefabs[tempIndex].name);
            currentGunInstance = Instantiate(
                gunPrefabs[tempIndex],
                spawnPoint.position,
                spawnPoint.rotation,
                spawnPoint
            );
            if (gunPrefabs[tempIndex].name.Contains("ShotGun"))
            {
                currentGunInstance.transform.Rotate(0f, -90f, 0f); // adjust as needed
            }
            currentGunInstance.transform.localScale = Vector3.one * 3.5f;
            Debug.Log("Spawned gun: " + gunPrefabs[tempIndex].name);
            index++;
            timer += cycleInterval;

            yield return new WaitForSeconds(cycleInterval);
        }
        if (currentGunInstance != null)
            Destroy(currentGunInstance);

        int finalIndex = Random.Range(0, gunPrefabs.Count);
        currentGunInstance = Instantiate(
            gunPrefabs[finalIndex],
            spawnPoint.position,
            spawnPoint.rotation,
            spawnPoint
        );
        if (gunPrefabs[finalIndex].name.Contains("ShotGun"))
        {
            currentGunInstance.transform.Rotate(0f, -90f, 0f); // adjust as needed
        }
        currentGunInstance.transform.localScale = Vector3.one * 3.5f;
        Debug.Log("Final gun selected: " + currentGunInstance.name);
        // You can do more here if you want to give the gun to the player, etc.
    }
}
