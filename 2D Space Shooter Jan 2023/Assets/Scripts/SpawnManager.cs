using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    float _yPositionLimit = 6f;
    float _xPositionLimit = 9.0f;
    float _randomX;
    float _randomY;
    float _waitTime = 5.0f;
    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnRoutine()
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
            yield return new WaitForSeconds(_waitTime);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
