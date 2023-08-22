using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerupPrefab;
    [SerializeField] private GameObject[] _powerupPrefabRare;
    [SerializeField] private float _waitTimePowerupsNormalMin = 2.0f;
    [SerializeField] private float _waitTimePowerupsNormalMax = 5.0f;
    [SerializeField] private float _waitTimePowerupsRareMin = 10.0f;
    [SerializeField] private float _waitTimePowerupsRareMax = 20.0f;

    float _yPositionLimit = 6f;
    float _xPositionLimit = 9.0f;
    float _randomX;
    float _waitTimeEnemy = 5.0f;
    float _waitTimePowerups = 7.0f; // In between powerup spawning
    private bool _stopSpawning = false;
    private GameManager _gameManager;
    float _randomZAngle;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("SpawnManager::Start(). Game Manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine(_powerupPrefab, _waitTimePowerupsNormalMin, _waitTimePowerupsNormalMax));
        StartCoroutine(SpawnPowerupRoutine(_powerupPrefabRare, _waitTimePowerupsRareMin, _waitTimePowerupsRareMax));
    }

    IEnumerator InitialPowerupsDelay()
    {
        yield return new WaitForSeconds(10.0f);
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
        {
            // Instantiate enemy prefab
            _randomX = Random.Range(-_xPositionLimit, _xPositionLimit);
            _randomZAngle =  Random.Range(-45f,45f);
            Vector3 spawnPosition = new Vector3(_randomX, _yPositionLimit, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.Euler(0,0, _randomZAngle));
            newEnemy.transform.parent = _enemyContainer.transform;

            // yield wait for 5 seconds
            yield return new WaitForSeconds(_waitTimeEnemy);
        }
    }

    IEnumerator SpawnPowerupRoutine(GameObject[] _spawnList, float _waitTimeMin, float _waitTimeMax)
    {
        int _randomPowerUpIndex = 0;

        while (_stopSpawning == false)
        {
            _waitTimePowerups = Random.Range(_waitTimeMin, _waitTimeMax);

            yield return new WaitForSeconds(_waitTimePowerups);
            
            // Instantiate prowerup prefab
            _randomX = Random.Range(-_xPositionLimit, _xPositionLimit);
            Vector3 spawnPosition = new Vector3(_randomX, _yPositionLimit, 0);
            _randomPowerUpIndex = Random.Range(0, _spawnList.Length);
            GameObject newPowerup = Instantiate(_spawnList[_randomPowerUpIndex], spawnPosition, Quaternion.identity);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
