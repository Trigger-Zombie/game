using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject beamPrefab;
    public Transform beamSpawnPoint;  // 👈 NEW: Drag BeamSpawnPoint into this in the Inspector
    public float spawnDelay = 5f;
    public float objectiveTime = 20f;

    private GameObject currentBeam;
    private float timer;
    private bool timerRunning = false;

    void Start()
    {
        Invoke("SpawnBeam", spawnDelay);
    }

    void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                Destroy(currentBeam);
                timerRunning = false;
                Debug.Log("Objective failed (timer ran out).");
            }
        }
    }

    void SpawnBeam()
    {
        if (beamSpawnPoint == null)
        {
            Debug.LogWarning("Beam spawn point is not assigned!");
            return;
        }

        currentBeam = Instantiate(beamPrefab, beamSpawnPoint.position, beamSpawnPoint.rotation);
        timer = objectiveTime;
        timerRunning = true;
    }
}
