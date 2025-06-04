using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("Spawner Setup")]
    public List<enemy_spawner> spawners;

    [Header("Objective System")]
    public ObjectiveManager objectiveManager;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public float timeBetweenWaves = 20f;

    [Header("UI")]
    public TextMeshProUGUI waveText;

    [Header("Wave Audio")]
    public AudioSource audioSource;
    public AudioClip waveStartSound;

    [Header("Zombie Grunt Audio")]
    public AudioClip[] zombieGruntSounds;
    public float gruntInterval = 5f;
    private float gruntTimer = 0f;

    private int enemiesAlive = 0;
    private bool waveActive = false;
    private bool waitingForNextWave = false;
    private float nextWaveCountdown = 0f;

    void Start()
    {
        UpdateWaveText(0);
        StartNewWave();
    }

    void Update()
    {
        // Handle grunt sounds if enemies are alive
        if (enemiesAlive > 0 && audioSource != null && zombieGruntSounds.Length > 0)
        {
            gruntTimer += Time.deltaTime;
            if (gruntTimer >= gruntInterval)
            {
                gruntTimer = 0f;
                int index = Random.Range(0, zombieGruntSounds.Length);
                audioSource.PlayOneShot(zombieGruntSounds[index]);
                Debug.Log("Grunt sound played: " + zombieGruntSounds[index].name);
            }
        }
        else
        {
            gruntTimer = 0f;
        }
    }

    void UpdateWaveText(int waveNumber, float countdown = -1f)
    {
        if (waveText != null)
        {
            string text = $"Wave: {waveNumber}";
            if (countdown > 0f)
            {
                text += $"\n     {Mathf.CeilToInt(countdown)}";
            }
            waveText.text = text;
        }
    }

    void StartNewWave()
    {
        if (audioSource != null && waveStartSound != null)
        {
            audioSource.PlayOneShot(waveStartSound);
        }

        waveActive = true;
        currentWave++;

        UpdateWaveText(currentWave); // no countdown here

        int totalEnemies = GetEnemyCountForWave(currentWave);

        if (objectiveManager != null)
        {
            StartCoroutine(DelayedObjectiveSpawn());
        }

        StartCoroutine(SpawnEnemies(totalEnemies));
    }

    IEnumerator DelayedObjectiveSpawn()
    {
        yield return new WaitForSeconds(7f);
        if (objectiveManager != null)
        {
            objectiveManager.SpawnNextObjective();
        }
    }

    IEnumerator SpawnEnemies(int totalEnemies)
    {
        int spawned = 0;

        while (spawned < totalEnemies)
        {
            if (spawners.Count > 0)
            {
                enemy_spawner chosenSpawner = spawners[Random.Range(0, spawners.Count)];
                GameObject enemy = chosenSpawner.SpawnOneEnemy();

                if (enemy != null)
                {
                    EnemyMovement zombieScript = enemy.GetComponent<EnemyMovement>();
                    if (zombieScript != null)
                    {
                        zombieScript.waveManager = this;
                    }

                    zombieHitbox zHit = enemy.GetComponentInChildren<zombieHitbox>();
                    if (zHit != null)
                    {
                        zHit.waveManager = this;
                    }

                    ghostHitBox gHit = enemy.GetComponentInChildren<ghostHitBox>();
                    if (gHit != null)
                    {
                        gHit.waveManager = this;
                    }
                    
                    TankHitBox tankHit = enemy.GetComponentInChildren<TankHitBox>();
                    if (tankHit != null)
                    {
                        tankHit.waveManager = this;
                    }

                    enemiesAlive++;
                }

                spawned++;
            }

            yield return new WaitForSeconds(0.5f);
        }

        waveActive = false;
    }

    public void EnemyDied()
    {
        enemiesAlive--;
        Debug.Log($"Enemy died. Enemies remaining: {enemiesAlive}");

        if (enemiesAlive <= 0 && !waitingForNextWave)
        {
            Debug.Log("All enemies dead. Starting next wave countdown...");
            StartCoroutine(NextWaveCountdown());
        }
    }

    IEnumerator NextWaveCountdown()
    {
        waitingForNextWave = true;
        nextWaveCountdown = timeBetweenWaves;

        while (nextWaveCountdown > 0f)
        {
            UpdateWaveText(currentWave, nextWaveCountdown);
            nextWaveCountdown -= Time.deltaTime;
            yield return null;
        }

        waitingForNextWave = false;
        StartNewWave();
    }

    int GetEnemyCountForWave(int waveNumber)
    {
        if (waveNumber == 1)
            return 5;
        else if (waveNumber == 2)
            return 6;
        else if (waveNumber == 3)
            return 7;
        else
            return 6 + ((waveNumber - 3) * 2);
    }
}
