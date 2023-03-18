using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _powerups;

    float _yPositionLimit = 6f;
    float _xPositionLimit = 9.0f;
    float _randomX;
    float _randomY;
    float _waitTimeEnemy = 5.0f;
    float _waitTimePowerups = 7.0f;
    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
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
            _randomY = Random.Range(0, _yPositionLimit);
            Vector3 spawnPosition = new Vector3(_randomX, _randomY, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            // yield wait for 5 seconds
            yield return new WaitForSeconds(_waitTimeEnemy);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {


        yield return new WaitForSeconds(_waitTimePowerups);
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
