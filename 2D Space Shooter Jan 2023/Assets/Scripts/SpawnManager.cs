using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
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
    float _waitTimePowerups = 7.0f; // In between powerup spawning
    private bool _stopSpawning = false;
    private GameManager _gameManager;
    float _spawnZAngle;

    float _waitTimeEnemy = 5.0f; // Enemy spawning looping wait time between individual enemies
    float _waitTimeWaves = 7.0f; // Waves spawning looping wait time between waves of enemies
    int _maxEnemies = 1;
    int _enemiesSpawned = 0;


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

    public void StartSpawning(int _waveID)
    {
        Debug.Log("SpawnManager::StartSpawning() Started");
        _stopSpawning = false;
        GetWaveInfo(_waveID);
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine(_powerupPrefab, _waitTimePowerupsNormalMin, _waitTimePowerupsNormalMax));
        StartCoroutine(SpawnPowerupRoutine(_powerupPrefabRare, _waitTimePowerupsRareMin, _waitTimePowerupsRareMax));
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

    private void GetWaveInfo(int _waveID)
    {
        Debug.Log("SpawnManager::GetWaveInfo() Called");
        WaitForSeconds _respawnTime = new WaitForSeconds(10);


        switch (_waveID)
        {
            case 1:
                _maxEnemies = 2;
                _waitTimeEnemy = 3.5f;
                break;
            case 2:
                _maxEnemies = 3;
                _waitTimeEnemy = 3.0f;
                break;
            case 3:
                _maxEnemies = 4;
                _waitTimeEnemy = 2.5f;
                break;
            case 4:
                _maxEnemies = 6;
                _waitTimeEnemy = 2.0f;
                break;
            case 5:
                _maxEnemies = 8;
                _waitTimeEnemy = 1.0f;
                break;
            case 6:
                _maxEnemies = 4;
                _waitTimeEnemy = 0.5f;
                break;
        }

    }

    IEnumerator SpawnEnemyRoutine()
    {
        Debug.Log("SpawnManager::spawnEnemyRoutine() Called");

        int _enemyIndex = 0; // Initialized to Standard Enemy

        while (_stopSpawning == false)
        {
            for (int i = 0; i < _maxEnemies; i++)
            {
                yield return new WaitForSeconds(_waitTimeEnemy);

                if (_stopSpawning == false)
                {
                    // Instantiate enemy prefab
                    _randomX = Random.Range(-_xPositionLimit, _xPositionLimit);
                    _spawnZAngle =  Random.Range(-45f,45f);
                    Vector3 spawnPosition = new Vector3(_randomX, _yPositionLimit, 0);

                    if (_gameManager._waveID > 5) // *** After wave 2, include spawning new enemy type
                    {
                        _enemyIndex = Random.Range(0, _enemyPrefab.Length);
                    }
                    if (_gameManager._waveID < 3) // Less than wave 3, no angle yet for simple enemies 
                    {
                        _spawnZAngle = 0;
                    }
                    
                    if (_enemyIndex == 1) // New enemy can spawn from the sides
                    {
                        spawnPosition.x = -_xPositionLimit;
                        spawnPosition.y = Random.Range(0, _yPositionLimit);
                        _spawnZAngle = 90;
                    }

                    GameObject newEnemy = Instantiate(_enemyPrefab[_enemyIndex], spawnPosition, Quaternion.Euler(0, 0, _spawnZAngle)); // Quaternion.Euler(0,0, _spawnZAngle)
                    newEnemy.transform.parent = _enemyContainer.transform;
                }

            }

            // yield wait for 5 seconds
            yield return new WaitForSeconds(_waitTimeWaves);
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
