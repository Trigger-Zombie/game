using System.Collections.Generic;
using UnityEngine;

public class enemy_spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemyPrefabs; // Drag 8 zombies + 2 ghosts here

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
}
