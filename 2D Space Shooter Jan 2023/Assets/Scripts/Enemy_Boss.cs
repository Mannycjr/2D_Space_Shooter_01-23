using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;

    private GameManager _gameManager;
    private SpawnManager _spawnManager;

    GameObject _laserSpawnPoint;
    GameObject _laserSpawnPointBack;
    private float _verticalLimit = 7.0f;
    private float _horizontalLimit = 11.0f;

    int _bossHealthMax = 100;
    int _bossHealthCurrent = 0;

    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Enemy::Start() No _player");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Enemy::Start(). Game Manager is NULL");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Enemy::Start(). Spawn Manager is NULL");
        }

        _bossHealthCurrent = _bossHealthMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DamageEnemy()
    {
        
        if ((_bossHealthCurrent <= _bossHealthMax) && (_bossHealthCurrent > 0))
        {
            //ExplosionOnlyAnim();
            //ShieldsNOTActive();
        }
        else
        {
            //DestoryEnemy();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Enemy::OnTriggerEnter2D:Begin");
        if (other.tag == "Player")
        {
            // Damage the player
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            DamageEnemy();
        }
        else if (other.tag == "Laser")
        {

            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.PlayerScoreUpdate(10);
            }
            DamageEnemy();
        }
        // Any other collision will not damage enemy
    }
}
