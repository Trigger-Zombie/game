using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject beamPrefab;
    public List<Transform> beamSpawnPoints;  // 🔁 New list of spawn points
    public float spawnDelay = 5f;
    public float objectiveTime = 25f;

    public GameObject objectiveUI;
    public TextMeshProUGUI objectiveText;

    private string baseObjectiveText;
    private GameObject currentBeam;
    private float timer;
    private bool timerRunning = false;

    private int currentObjectiveIndex = 0;

    void Start()
    {
        if (objectiveText != null)
            baseObjectiveText = objectiveText.text;

    }

    void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;

            if (objectiveText != null)
                objectiveText.text = $"{baseObjectiveText}\nTime Left: {Mathf.CeilToInt(timer)}";

            if (timer <= 0f)
            {
                Destroy(currentBeam);
                timerRunning = false;
                Debug.Log("Objective failed.");

                if (objectiveUI != null)
                    objectiveUI.SetActive(false);

                if (objectiveText != null)
                    objectiveText.text = baseObjectiveText;
            }
        }
    }

    public void SpawnNextObjective()
    {
        if (beamSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No beam spawn points assigned!");
            return;
        }

        // Wrap around when out of bounds
        int index = currentObjectiveIndex % beamSpawnPoints.Count;
        Transform spawnPoint = beamSpawnPoints[index];

        if (spawnPoint == null || beamPrefab == null)
        {
            Debug.LogWarning("Missing spawn point or beam prefab!");
            return;
        }

        currentBeam = Instantiate(beamPrefab, spawnPoint.position, spawnPoint.rotation);
        BeamObjective beamScript = currentBeam.GetComponent<BeamObjective>();

        if (beamScript != null && objectiveUI != null)
            beamScript.objectiveUI = objectiveUI;

        if (objectiveUI != null)
            objectiveUI.SetActive(true);

        if (objectiveText != null)
            objectiveText.text = $"{baseObjectiveText}\nTime Left: {Mathf.CeilToInt(objectiveTime)}s";

        timer = objectiveTime;
        timerRunning = true;

        currentObjectiveIndex++; // ✅ Always increment
    }
}
