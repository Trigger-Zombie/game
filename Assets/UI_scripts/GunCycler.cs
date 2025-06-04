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

    //private GameObject currentGunInstance;

    [HideInInspector] public GameObject finalGunPrefab;     // The prefab to give the player
    [HideInInspector] public GameObject currentGunInstance;

    public void StartCycling()
    {
        // Hide cube's visual
        Debug.Log("Hiding cube");
        Transform visualMesh = transform.Find("Cube");
        if (visualMesh != null)
        {
            visualMesh.gameObject.SetActive(false);
        }
        StartCoroutine(CycleGuns());
    }

    
    IEnumerator CycleGuns()
    {
        float timer = 0f;
        int index = 0;

        // Loop through guns visually
        while (timer < totalCycleTime)
        {
            if (currentGunInstance != null)
                Destroy(currentGunInstance);

            int tempIndex = index % gunPrefabs.Count;
            currentGunInstance = Instantiate(
                gunPrefabs[tempIndex],
                spawnPoint.position,
                spawnPoint.rotation,
                spawnPoint
            );

            //if (gunPrefabs[tempIndex].name.Contains("ShotGun"))
            //{
            //    currentGunInstance.transform.Rotate(0f, -90f, 0f); // adjust if needed
            //}

            currentGunInstance.transform.localScale = Vector3.one * 3.5f;
            index++;
            timer += cycleInterval;

            yield return new WaitForSeconds(cycleInterval);
        }

        // Cleanup last temp gun
        if (currentGunInstance != null)
            Destroy(currentGunInstance);

        // Choose final gun to give the player
        int finalIndex = Random.Range(0, gunPrefabs.Count);
        finalGunPrefab = gunPrefabs[finalIndex];

        currentGunInstance = Instantiate(
            finalGunPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            spawnPoint
        );

        // Apply visual rotation for preview only — finalGunPrefab stays untouched
        //if (finalGunPrefab.name.Contains("ShotGun"))
        //{
        //    currentGunInstance.transform.localRotation *= Quaternion.Euler(0f, -90f, 0f);
        //}

        currentGunInstance.transform.localScale = Vector3.one * 3.5f;

        Debug.Log("Final gun selected: " + finalGunPrefab.name);
    }
}
