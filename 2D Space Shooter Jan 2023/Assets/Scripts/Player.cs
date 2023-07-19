using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speedDefault = 3.5f;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private int _lives = 3;
    private Vector3 _initPosition = new Vector3(0, -3.8f, 0);

    [SerializeField] private float verticalLimit = 5.0f;
    [SerializeField] private float horizontalLimit = 11.0f;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotlaserPrefab;
    private Vector3 _laserOffset = new Vector3(0, 1.05f, 0);
    [SerializeField] private float _firerate = 0.15f;
    private float _canFire = -1.0f;

    SpawnManager _spawnManager;

    [SerializeField] private float _speedBoostMultiplierShift = 2.5f;

    //Powerups variables
    [SerializeField] private bool _tripleShotActive = false;
    [SerializeField] private bool _speedBoostPowerupActive = false;
    [SerializeField] private bool _speedBoostShiftActive = false;
    [SerializeField] private bool _shieldsActiveAlready = false;
    [SerializeField] private int _shieldStrength = 3;
    [SerializeField] private GameObject _shieldsPlayer;
    [SerializeField] private float _speedBoostMultiplierPowerup = 7.0f;

    private int _score = 0;
    UIManager _UIManager;

    //VFX
    [SerializeField] private GameObject _damageSmokeLeft, _damageSmokeRight;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;

    //SFX
    [SerializeField] private AudioClip _laserShotAudioClip;
    private AudioSource _sfxAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        // take the current position = new position (0,0,0)
        transform.position = _initPosition;
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Player::Start:No _spawnManager");
        }

        if (_UIManager == null)
        {
            Debug.LogError("Player::Start:No _UIManager");
        }

        _sfxAudioSource = GetComponent<AudioSource>();
        if (_sfxAudioSource == null)
        {
            Debug.LogError("No laser sound");
        }
        else
        {
            _sfxAudioSource.clip = _laserShotAudioClip;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SpeedBoostActiveShift();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            SpeedReset();
        }

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

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

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

        _sfxAudioSource.clip = _laserShotAudioClip;
        _sfxAudioSource.Play();
    }

    public void Damage()
    {
        if (!_shieldsActiveAlready)
        {
            _lives--;

            _UIManager.UpdateLives(_lives);
            VFX_Smoke();

            if (_lives < 1)
            {
                ExplosionAnim();
                _spawnManager.OnPlayerDeath();
                _UIManager.GameOverSequence();
                Destroy(this.gameObject);
            }
        }
        else // _shieldsActiveAlready = true
        {    
            _shieldsActiveAlready = false;
            _shieldsPlayer.SetActive(false);
        }


    }

    private void VFX_Smoke()
    {
        if (_lives == 2)
        {
            _damageSmokeLeft.SetActive(true);
            _damageSmokeRight.SetActive(false);
        } 
        else if (_lives == 1)
        {
            _damageSmokeLeft.SetActive(true);
            _damageSmokeRight.SetActive(true);
        }
        else
        {
            _damageSmokeLeft.SetActive(false);
            _damageSmokeRight.SetActive(false);
        }
    }

    public void TripleShotActive(float _duration)
    {
        _tripleShotActive = true;
        StartCoroutine(TripleShotDuration(_duration));
    }

    IEnumerator TripleShotDuration(float delay)
    {
        yield return new WaitForSeconds(delay);
        _tripleShotActive = false;
    }

    public void SpeedBoostActive(float _duration)
    {
        _speedBoostPowerupActive = true;
        _speed = _speedDefault * _speedBoostMultiplierPowerup;
        StartCoroutine(SpeedBoostDurationCoroutine(_duration));
    }

    IEnumerator SpeedBoostDurationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpeedReset();
    }

    public void SpeedBoostActiveShift()
    {
        if (_speedBoostPowerupActive == false)
        {
            _speedBoostShiftActive = true;
            _speed = _speedDefault * _speedBoostMultiplierShift;
        }

    }

    public void SpeedReset ()
    {
        _speed = _speedDefault;
        _speedBoostPowerupActive = false;
        _speedBoostShiftActive = false;
    }

    public void ShieldsActive(float _duration)
    {
        _shieldsActiveAlready = true;
        _shieldsPlayer.SetActive(true);
        StartCoroutine(ShieldsActivateDurationCoroutine(_duration));
    }

    IEnumerator ShieldsActivateDurationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public void PlayerScoreUpdate(int points)
    {
        _score += points;
        _UIManager.UpdateScore(_score);
    }

    private void ExplosionAnim()
    {
        _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(_explosionInstance, 2.7f);
    }

}
