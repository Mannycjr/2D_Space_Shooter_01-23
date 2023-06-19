using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private float _rotationSpeed = 20.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;
    private SpawnManager _spawnManager;
    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Asteroid::Start: Begin.");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Asteroid::Start. Spawn Manager is NULL.");
        }

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_spawnManager == null)
        {
            Debug.LogError("Asteroid::Start. Player is NULL.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit! Asteroid::OnTriggerEnter: Begin. other.tag=" + other.tag);
        if (other.tag == "Laser" ) 
        {
            Debug.Log("Asteroid::OnTriggerEnter: if other.tag=" + other.tag);
            Destroy(other.gameObject);
            ExplosionAnim();

            _spawnManager.StartSpawning();
    
        }
        else if (other.tag == "Player")
        {
            ExplosionAnim();
            _player.Damage();

            _spawnManager.StartSpawning();
        }


        Destroy(this.gameObject,0.0f);
    }

    private void ExplosionAnim()
    {
        _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(_explosionInstance, 2.5f);
    }
}
