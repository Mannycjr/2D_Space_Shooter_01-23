using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerupPrefabFrequent;
    [SerializeField] private GameObject[] _powerupPrefab;
    [SerializeField] private GameObject[] _powerupPrefabRare;
    [SerializeField] private float _waitTimePowerupsFrequentMin = 2.0f;
    [SerializeField] private float _waitTimePowerupsFrequentMax = 5.0f;
    [SerializeField] private float _waitTimePowerupsNormalMin = 5.0f;
    [SerializeField] private float _waitTimePowerupsNormalMax = 15.0f;
    [SerializeField] private float _waitTimePowerupsRareMin = 15.0f;
    [SerializeField] private float _waitTimePowerupsRareMax = 25.0f;

    public int afterLevelXNewEnemySpawn = 3;
    public int afterLevelXNewEnemyAngledSpawned = 6;

    float _yPositionLimit = 6f;
    float _xPositionLimit = 9.0f;
    float _randomX;
    float _waitTimePowerups = 7.0f; // In between powerup spawning
    private bool _stopSpawning = false;
    private GameManager _gameManager;
    float _spawnZAngle = 0;

    float _waitTimeEnemy = 5.0f; // Enemy spawning looping wait time between individual enemies
    float _waitTimeWaves = 7.0f; // Waves spawning looping wait time between waves of enemies
    int maxEnemiesSpawned = 1;

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

    public void StartSpawning(int waveID)
    {
        Debug.Log("SpawnManager::StartSpawning() Started");
        _stopSpawning = false;
        GetWaveInfo(waveID);
        StartCoroutine(SpawnEnemyRoutine());
        if (waveID < 2) // Start spawning powerup coroutines only in wave 1
        {
            StartCoroutine(SpawnPowerupRoutine(_powerupPrefabFrequent, _waitTimePowerupsFrequentMin, _waitTimePowerupsFrequentMax));
            StartCoroutine(SpawnPowerupRoutine(_powerupPrefab, _waitTimePowerupsNormalMin, _waitTimePowerupsNormalMax));
            StartCoroutine(SpawnPowerupRoutine(_powerupPrefabRare, _waitTimePowerupsRareMin, _waitTimePowerupsRareMax));
        }
        if (waveID == 7) // Add another powerup coroutine to help the player at more difficult waves 
        {
            StartCoroutine(SpawnPowerupRoutine(_powerupPrefab, _waitTimePowerupsNormalMin, _waitTimePowerupsNormalMax));
        }
    }

    IEnumerator InitialPowerupsDelay()
    {
        yield return new WaitForSeconds(10.0f);
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
        ClearEnemies();
    }

    private void ClearEnemies()
    {
        Debug.Log("SpawnManager::ClearEnemies() Called");
        Enemy[] _activeEnemies = _enemyContainer.GetComponentsInChildren<Enemy>();

        foreach (Enemy _enemy in _activeEnemies)
        {
            _enemy.ClearField();
        }
    }

    private void GetWaveInfo(int waveID)
    {
        switch (waveID)
        {
            case 1:
                maxEnemiesSpawned = 2;
                _waitTimeEnemy = 3.5f;
                break;
            case 2:
                maxEnemiesSpawned = 2;
                _waitTimeEnemy = 3.0f;
                break;
            case 3:
                maxEnemiesSpawned = 3;
                _waitTimeEnemy = 2.5f;
                break;
            case 4:
                maxEnemiesSpawned = 2;
                _waitTimeEnemy = 2.0f;
                break;
            case 5:
                maxEnemiesSpawned = 1;
                _waitTimeEnemy = 1.0f;
                break;
            case 6:
                maxEnemiesSpawned = 1;
                _waitTimeEnemy = 0.5f;
                break;
            case 7:
                maxEnemiesSpawned = 2;
                _waitTimeEnemy = 3.5f;
                break;
            case 8:
                maxEnemiesSpawned = 3;
                _waitTimeEnemy = 4.0f;
                break;
            case 9:
                maxEnemiesSpawned = 4;
                _waitTimeEnemy = 6.0f;
                break;
            case 10:
                maxEnemiesSpawned = 5;
                _waitTimeEnemy = 10.0f;
                break;
        }

    }

    IEnumerator SpawnEnemyRoutine()
    {
        int _enemyIndex = 0; // Initialized to Standard Enemy

        while ((_stopSpawning == false) && (_gameManager._isGameOver == false))
        {
            for (int i = 0; i < maxEnemiesSpawned; i++)
            {
                yield return new WaitForSeconds(_waitTimeEnemy);

                if ((_stopSpawning == false) && (_gameManager._isGameOver == false))
                {
                    // Instantiate enemy prefab
                    _randomX = Random.Range(-_xPositionLimit, _xPositionLimit);
                    
                    Vector3 spawnPosition = new Vector3(_randomX, _yPositionLimit, 0);

                    if (_gameManager.waveID > afterLevelXNewEnemySpawn) // After wave 3 (default), include spawning new enemy type
                    {
                        _enemyIndex = Random.Range(0, _enemyPrefab.Length);
                    }
                    if (_gameManager.waveID > afterLevelXNewEnemyAngledSpawned) // After wave 6 (default), different spawn angles for ALL enemies 
                    {
                        _spawnZAngle = Random.Range(-45f, 45f);
                    }
                    // Note: Laser Beam Enemy Wavy Movement set in Enemy.cs in afterLevelXLaserBeamEnemyWavyMove
                    GameObject newEnemy = Instantiate(_enemyPrefab[_enemyIndex], spawnPosition, Quaternion.Euler(0, 0, _spawnZAngle));
                    newEnemy.transform.parent = _enemyContainer.transform;
                    if ((_stopSpawning == true) && (_gameManager._isGameOver == true))
                    {
                        yield break;
                    }
                }

            }

            // yield wait for 5 seconds
            yield return new WaitForSeconds(_waitTimeWaves);
        }
    }



    IEnumerator SpawnPowerupRoutine(GameObject[] _spawnList, float _waitTimeMin, float _waitTimeMax)
    {
        int _randomPowerUpIndex = 0;

        while ((_stopSpawning == false) && (_gameManager._isGameOver == false))
        {
            _waitTimePowerups = Random.Range(_waitTimeMin, _waitTimeMax);

            yield return new WaitForSeconds(_waitTimePowerups);

            if ((_stopSpawning == false) && (_gameManager._isGameOver == false))
            {
                // Instantiate prowerup prefab
                _randomX = Random.Range(-_xPositionLimit, _xPositionLimit);
                Vector3 spawnPosition = new Vector3(_randomX, _yPositionLimit, 0);
                _randomPowerUpIndex = Random.Range(0, _spawnList.Length);
                GameObject newPowerup = Instantiate(_spawnList[_randomPowerUpIndex], spawnPosition, Quaternion.identity);
            }
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        _gameManager.GameOver();
    }

}
