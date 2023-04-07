using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speedDefault = 3.5f;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private int _lives = 3;
    private Vector3 _initPosition = new Vector3(0, -3.8f, 0);

    private float horizontalInput;
    private float verticalInput;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotlaserPrefab;
    private Vector3 _laserOffset = new Vector3(0, 1.05f, 0);
    [SerializeField] private float _firerate = 0.15f;
    private float _canFire = -1.0f;

    SpawnManager _spawnManager;

    //Powerups variables
    [SerializeField] private bool _tripleShotActive = false;

    [SerializeField] private bool _speedBoostActive = false;
    [SerializeField] private float _speedBoostMultiplier = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        // take the current position = new position (0,0,0)
        transform.position = _initPosition;
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Player::Start:No _spawnManager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire )
        {
            Firelaser();
        }
    }



    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float verticalLimit = 5.0f;
        float horizontalLimit = 11.0f;

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        Debug.Log("Player::CalculateMovement::_speedBoostActive=" + _speedBoostActive);
        // SpeedBoost Logic
        if (_speedBoostActive)
        {
            _speed = _speedDefault * _speedBoostMultiplier;
        }
        else
        {
            _speed = _speedDefault;
        }
        Debug.Log("Player::CalculateMovement::_speed="+ _speed);

        // Movement
        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -verticalLimit, verticalLimit), 0);

        if (transform.position.x >= horizontalLimit)
        {
            transform.position = new Vector3(-horizontalLimit, transform.position.y, 0);
        }
        else if (transform.position.x <= -horizontalLimit)
        {
            transform.position = new Vector3(horizontalLimit, transform.position.y, 0);
        }
    }

    void Firelaser()
    {
        Vector3 _laserPosition = transform.position + _laserOffset;

        _canFire = Time.time + _firerate;

        if (_tripleShotActive)
        {
            Instantiate(_tripleShotlaserPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, _laserPosition, Quaternion.identity);
        }

        
    }

    public void Damage()
    {
        _lives--;

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive(float _duration)
    {
        _tripleShotActive = true;
        StartCoroutine(TripleShotDuration(_duration));
    }

    IEnumerator TripleShotDuration(float _duration)
    {
        yield return new WaitForSeconds(_duration);
        _tripleShotActive = false;
    }

    public void SpeedBoostActive(float _duration)
    {
        _speedBoostActive = true;
        StartCoroutine(SpeedBoostDurationCoroutine(float _duration));
    }

    IEnumerator SpeedBoostDurationCoroutine(float _duration)
    {
        yield return new WaitForSeconds(_duration);
        _speedBoostActive = false;
    }
}
