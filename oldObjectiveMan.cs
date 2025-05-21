using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject beamPrefab;
    public Transform beamSpawnPoint;
    public float spawnDelay = 5f;
    public float objectiveTime = 20f;

    public GameObject objectiveUI; // for objective panel
    public TextMeshProUGUI objectiveText; // the text that shows the message
    private string baseObjectiveText;

    private GameObject currentBeam;
    private float timer;
    private bool timerRunning = false;

    void Start()
    {
        if (objectiveText != null)
        {
            baseObjectiveText = objectiveText.text; // Store the starting message
        }

        Invoke("SpawnBeam", spawnDelay);
    }

    void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;

            if (objectiveText != null)
            {
                int secondsLeft = Mathf.CeilToInt(timer);
                objectiveText.text = $"{baseObjectiveText}\nTime Left: {secondsLeft}";
            }

            if (timer <= 0f)
            {
                Destroy(currentBeam);
                timerRunning = false;
                Debug.Log("Objective failed (timer ran out).");

                if (objectiveUI != null)
                {
                    objectiveUI.SetActive(false);
                }

                if (objectiveText != null)
                {
                    objectiveText.text = baseObjectiveText; // Reset to original
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
            objectiveUI.SetActive(true);
        }

        if (objectiveText != null)
        {
            objectiveText.text = $"{baseObjectiveText}\nTime Left: {Mathf.CeilToInt(objectiveTime)}s";
        }

        timer = objectiveTime;
        timerRunning = true;
    }
}
