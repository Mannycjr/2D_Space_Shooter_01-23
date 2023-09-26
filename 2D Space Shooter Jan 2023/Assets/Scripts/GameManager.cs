using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool _isGameOver = true;
    private UIManager _uiManagerScript;

    private SpawnManager _spawnManager;
    public int waveID = 0;
    private float _waveTime = 5.0f;
    private float _holdTime = 2.0f;

    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("GameManager::Start() Called. The Spawn Manager is NULL");

        }

        _uiManagerScript = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManagerScript == null)
        {
            Debug.LogError("GameManager::Start() Called. The UI Manager is NULL");

        }        
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainGame_Scene" && _isGameOver == true)
        {
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(1); // Reloads same scene, Game scene
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene(0); // Loads Main Menu
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // 
        {
            Debug.Log("ESC General");
            Application.Quit();
        }

    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public void GameNotOver()
    {
        _isGameOver = false;
    }

    public void StartSpawning()
    {
        waveID++;
        _waveTime += 10f;

        if (waveID > 6)
        {
            Debug.Log("You Win!");
            return;
        }

        _uiManagerScript.WaveDisplayOn();
        _uiManagerScript.WaveIDUpdate(waveID);
        StartCoroutine(WaveCountdown(_waveTime));
        _spawnManager.StartSpawning(waveID);
    }

    private IEnumerator WaveCountdown(float _time)
    {
        while ((_time > 0 ) && (!_isGameOver))
        {
            _time -= Time.deltaTime;
            _uiManagerScript.WaveTimeUpdate(_time);
            yield return new WaitForEndOfFrame();
        }
        _spawnManager.StopSpawning();

        yield return _holdTime;
        StartSpawning();
    }
}
