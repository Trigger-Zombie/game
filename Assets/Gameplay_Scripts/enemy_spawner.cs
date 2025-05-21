using UnityEngine;

public class enemy_spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private float _minimumSpawnTime;
    [SerializeField]
    private float _maximumSpawnTime;

    private float _timeUnitlSpawn;


    void awake(){
        SetTimeUntilSpawn();
    }

    void Update()
    {
        _timeUnitlSpawn -= Time.deltaTime;
        if(_timeUnitlSpawn <= 0)
        {
            Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
            SetTimeUntilSpawn();
        }
    }
    private void SetTimeUntilSpawn()
    {
        _timeUnitlSpawn = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }
}