using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _powerupDuration = 7.0f;
    public enum _powerupIDs
    {
        TripleShot,
        SpeedBoost,
        Shields,
        Ammo,
        Health,
        WideShot,
        Homing,
        NoAmmo
    }


    [SerializeField] private _powerupIDs _powerupID;
    private float _verticalLimit = 7.0f;

    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;

    private SpawnManager _spawnManager; // get script SpawnManager of GameObject Spawn_Manager
    private bool _moveTowardsPlayer = false;
    private Player _player;

    // SFX
    [SerializeField] private AudioClip _powerupAudioClip;
    [SerializeField] private AudioClip _sfxClipExplosion;
    private float _audioPositionZ = -9f;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Powerup::Start: Spawn Manager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_moveTowardsPlayer)
        {
            // move down at a speed
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else if ((_moveTowardsPlayer) & (_player != null))
        {
            Debug.Log("MovingTowardPlayer");
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _speed * 2.0f * Time.deltaTime);
        }

        // when leave screen, destroy object
        if (transform.position.y <= -_verticalLimit)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // Enable the tripleshot lasers in player
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                if (_powerupID == _powerupIDs.NoAmmo)
                {
                    _audioPositionZ = -9.5f; // Closer to camera, a little louder
                }
                AudioSource.PlayClipAtPoint(_powerupAudioClip, new Vector3(0, 0, _audioPositionZ));

                switch (_powerupID)
                {
                    case _powerupIDs.TripleShot:
                        player.TripleShotActive(_powerupDuration);
                        Debug.Log("Powerup::OnTriggerEnter2D:switch TripleShot");
                        break;
                    case _powerupIDs.SpeedBoost:
                        player.SpeedBoostActive(_powerupDuration);
                        Debug.Log("Powerup::OnTriggerEnter2D:switch SpeedBoost");
                        break;
                    case _powerupIDs.Shields:
                        player.ShieldsActive();
                        Debug.Log("Powerup::OnTriggerEnter2D:switch Shields");
                        break;
                    case _powerupIDs.Ammo:
                        player.AmmoRefill();
                        Debug.Log("Powerup::OnTriggerEnter2D:switch Ammo");
                        break;
                    case _powerupIDs.Health:
                        player.AddShip();
                        Debug.Log("Powerup::OnTriggerEnter2D:switch AddLife");
                        break;
                    case _powerupIDs.WideShot:
                        player.WideShotActive(5.0f);
                        Debug.Log("Powerup::OnTriggerEnter2D:switch WideShot");
                        break;
                    case _powerupIDs.Homing:
                        player.HomingMissileActive();
                        Debug.Log("Powerup::OnTriggerEnter2D:switch Homing Missile");
                        break;
                    case _powerupIDs.NoAmmo:
                        player.NoAmmo();
                        Debug.Log("Powerup::OnTriggerEnter2D:switch NoAmmo");
                        break;
                    default:
                        Debug.Log("Powerup::OnTriggerEnter2D:switch No_powerupID");
                        break;
                }

            }
            else
            {
                Debug.LogError("Powerup::OnTriggerEnter2D: player is null");
            }

            // 
            Destroy(this.gameObject);
        } else if (other.tag == "Laser")
        {
            AudioSource.PlayClipAtPoint(_sfxClipExplosion, new Vector3(0, 0, _audioPositionZ));
            ExplosionOnlyAnim();
            Destroy(this.gameObject);                
        }
    }

    private void ExplosionOnlyAnim()
    {
        _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(_explosionInstance, 4.0f);
    }

    public void MoveTowardsPlayerEnable()
    {
        _moveTowardsPlayer = true;
    }

    public void MoveTowardsPlayerDisable()
    {
        _moveTowardsPlayer = false;
    }
}
