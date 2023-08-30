using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    private float _verticalLimit = 7.0f;
    private float _horizontalLimit = 11.0f;

    private Player _player;
    // handle to animator component
    private Animator _enemyAnimator;
    private float _explosionAnimLength = 2.6f;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _sfxClipExplosion;
    [SerializeField] private AudioClip _sfxClipLaserSmall;

    [SerializeField] private GameObject _laserPrefab;
    private Vector3 _laserOffset = new Vector3(0, -1.15f, 0);

    private float _fireRate = 3.0f;
    private float _canFireAtTime = -1;

    private bool _isDestroyed = false;

    private bool _waveEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Enemy::Start() No _player");
        }

        _enemyAnimator = GetComponent<Animator>();
        if (_enemyAnimator == null)
        {
            Debug.LogError("Enemy::Start() No _enemyAnimator");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Enemy::Start() _audioSource is NULL.");
        }

        if (_laserPrefab == null)
        {
            Debug.LogError("Enemy::Start() _laserPrefab is NULL. Add prefab in Inspector.");
        }
        else
        {
            //_laserPrefab
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        FireLaser();

    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        float _randomXPos = Random.Range(-_horizontalLimit, _horizontalLimit);

        //if bottom of screen
        if (transform.position.y <= -_verticalLimit)
        {
            //respawn at top with a new random x position
            transform.position = new Vector3(_randomXPos, _verticalLimit, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // Damage the player
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            // trigger anim
            DestoryEnemy();
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.PlayerScoreUpdate(10);
            }
            DestoryEnemy();
        }

    }

    private void FireLaser()
    {
        //Debug.Log("Enemy::FireLaser: Begin");
        
        if (Time.time > _canFireAtTime && _isDestroyed == false)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFireAtTime = Time.time + _fireRate;
            Vector3 _laserPosition = transform.position + _laserOffset;


            //GameObject _enemyLaser = Instantiate(_laserPrefab, _laserPosition, Quaternion.identity);
            GameObject _enemyLaser = Instantiate(_laserPrefab, _laserPosition, transform.rotation);
            //_enemyLaser.transform.RotateAround(transform.position, Vector3.up, Quaternion.identity.eulerAngles.z);
            Laser[] lasers = _enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].EnemyLaser();

            }

            _audioSource.clip = _sfxClipLaserSmall;
            _audioSource.Play(0);
        }

    }

    private void DestoryEnemy()
    {
        _isDestroyed = true;
        _audioSource.clip = _sfxClipExplosion;
        _audioSource.Play(0);
        _enemyAnimator.SetTrigger("OnEnemyDeath"); // Explosion animaiton

        Destroy(GetComponent<Collider2D>()); // Do not collide any more
        _speed = 0; // No movement after shot

        Destroy(this.gameObject, _explosionAnimLength);
    }

    public void ClearField()
    {
        _canFireAtTime = -1;
        _waveEnded = true;
    }
}
