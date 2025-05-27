using UnityEngine;

public class enemy_spawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;

    public GameObject SpawnOneEnemy()
    {
        return Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
    }
}