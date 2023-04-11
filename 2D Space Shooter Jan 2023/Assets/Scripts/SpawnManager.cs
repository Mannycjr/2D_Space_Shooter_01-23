using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerupPrefab;

    float _yPositionLimit = 6f;
    float _xPositionLimit = 9.0f;
    float _randomX;
    float _randomY;
    float _waitTimeEnemy = 5.0f;
    float _waitTimePowerups = 7.0f; // In between powerup spawning
    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
        {
            // Instantiate enemy prefab
            _randomX = Random.Range(-_xPositionLimit, _xPositionLimit);
            Vector3 spawnPosition = new Vector3(_randomX, _yPositionLimit, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            // yield wait for 5 seconds
            yield return new WaitForSeconds(_waitTimeEnemy);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        int _randomPowerUpIndex = 0;

        while (_stopSpawning == false)
        {
            // spawn every 3-7 seconds
            _waitTimePowerups = Random.Range(1.0f, 3.0f);

            // Instantiate prowerup prefab
            _randomX = Random.Range(-_xPositionLimit, _xPositionLimit);
            Vector3 spawnPosition = new Vector3(_randomX, _yPositionLimit, 0);

            Debug.Log("SpawnManager::SpawnPowerupRoutine:_powerupPrefab.Length=" + (_powerupPrefab.Length));
            _randomPowerUpIndex = Random.Range(0, _powerupPrefab.Length); // 

            GameObject newPowerup = Instantiate(_powerupPrefab[_randomPowerUpIndex], spawnPosition, Quaternion.identity);
            Debug.Log("SpawnManager::SpawnPowerupRoutine: spawned _powerupPrefab[" + _randomPowerUpIndex + "]");

            yield return new WaitForSeconds(_waitTimePowerups);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
