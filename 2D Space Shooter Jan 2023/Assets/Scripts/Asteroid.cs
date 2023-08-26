using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;
    private SpawnManager _spawnManager;
    private Player _player;
    private GameManager _gameManager;

    void Start()
    {

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_spawnManager == null)
        {
            Debug.LogError("Asteroid::Start. Player is NULL.");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Asteroid::Start. Game Manager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit! Asteroid::OnTriggerEnter: Begin. other.tag=" + other.tag);
        if (other.tag == "Laser" ) 
        {
            //Debug.Log("Asteroid::OnTriggerEnter: if other.tag=" + other.tag);
            Destroy(other.gameObject);
            ExplosionAnim();
            _gameManager.StartSpawning();
    
        }
        else if (other.tag == "Player")
        {
            ExplosionAnim();
            _player.Damage();
            _gameManager.StartSpawning();
        }

        Destroy(this.gameObject,0.0f);
    }

    private void ExplosionAnim()
    {
        _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(_explosionInstance, 4.0f);
    }
}
