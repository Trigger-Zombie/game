using System.Collections.Generic;
using UnityEngine;

public class enemy_spawner : MonoBehaviour
{
    [SerializeField] public List<GameObject> _enemyPrefabs; // Drag 8 zombies + 2 ghosts here
    /*
    public GameObject SpawnOneEnemy()
    {
        if (_enemyPrefabs == null || _enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned!");
            return null;
        }

        int randomIndex = Random.Range(0, _enemyPrefabs.Count);
        return Instantiate(_enemyPrefabs[randomIndex], transform.position, Quaternion.identity);
    }
    */

    public GameObject SpawnOneEnemy()
    {
        if (_enemyPrefabs == null || _enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned!");
            return null;
        }

        int randomIndex = Random.Range(0, _enemyPrefabs.Count);
        GameObject prefab = _enemyPrefabs[randomIndex];

        // Look for custom spawn offset
        float yOffset = 0f;
        ghostspawnheight settings = prefab.GetComponent<ghostspawnheight>();
        if (settings != null)
        {
            yOffset = settings.spawnYOffset;
        }

        Vector3 spawnPos = transform.position + Vector3.up * yOffset;
        return Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}   
