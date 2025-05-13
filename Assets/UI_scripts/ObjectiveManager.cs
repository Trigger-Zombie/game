using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject beamPrefab;
    public Transform beamSpawnPoint;  
    public float spawnDelay = 5f;
    public float objectiveTime = 20f;

    private GameObject currentBeam;
    private float timer;
    private bool timerRunning = false;
    public GameObject objectiveUI;// for objective text top left
    // public CoinManager coinManager;

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

            if (objectiveUI != null)
            {
                objectiveUI.SetActive(false); // ✅ Hide objective UI if player fails
            }
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
        BeamObjective beamScript = currentBeam.GetComponent<BeamObjective>();
         if (beamScript != null && objectiveUI != null)
        {
            beamScript.objectiveUI = objectiveUI;
        }
        
        if (objectiveUI != null)
        {
            objectiveUI.SetActive(true); // ✅ Show the objective
        }
        // if (beamScript != null)
        // {
        //     beamScript.coinManager = coinManager;
        // }
        timer = objectiveTime;
        timerRunning = true;
    }
}
