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
    [SerializeField] private int _ammoCount = 15;

    SpawnManager _spawnManager;

    [SerializeField] private float _speedBoostMultiplierShift = 2.5f;

    //Powerups variables
    [SerializeField] private bool _tripleShotActive = false;
    [SerializeField] private bool _wideShotActive = false;
    [SerializeField] private bool _speedBoostPowerupActive = false;
    [SerializeField] private bool _speedBoostShiftActive = false;
    [SerializeField] private bool _shieldsActiveAlready = false;
    [SerializeField] private int _shieldStrength = 0;
    [SerializeField] private GameObject _shieldsOnPlayer;
    private SpriteRenderer _shieldsOnPlayerSpriteRenderer;
    [SerializeField] private float[] _shieldsScaling;
    [SerializeField] private Color[] _shieldsColor; // Double check user-set color's Alpha channel setting in Inspector.
    [SerializeField] private float _speedBoostMultiplierPowerup = 7.0f;

    private int _score = 0;
    UIManager _UIManager;

    //VFX
    [SerializeField] private GameObject _damageSmokeLeft, _damageSmokeRight;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;

    //SFX
    [SerializeField] private AudioClip _laserShotAudioClip;
    [SerializeField] private AudioClip _noAmmoAudioClip;
    private AudioSource _sfxAudioSource;

    //Thrusters
    [SerializeField] private float _powerupTimeLimit = 5.0f;
    [SerializeField] private float _powerupThrustersWaitTimeLimit = 3.0f;
    [SerializeField] private float _thrusterChargeLevelMax = 10.0f;
    [SerializeField] private float _thrusterChargeLevel;
    [SerializeField] private float _changeDecreaseThrusterChargeBy = 1.5f;
    [SerializeField] private float _changeIncreaseThrusterChargeBy = 0.01f;
    [SerializeField] private bool _canUseThrusters = true;
    [SerializeField] private bool _thrustersInUse = false;

    // Start is called before the first frame update
    void Start()
    {
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
        else
        {
            _UIManager.UpdateAmmo(_ammoCount);
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

        _shieldsOnPlayerSpriteRenderer = _shieldsOnPlayer.GetComponent<SpriteRenderer>();
        if (_shieldsOnPlayerSpriteRenderer == null)
        {
            Debug.LogError("Player::Start:No Sprite Renderer in Shields Game Object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check charge level. Restrict to min, max values
        _thrusterChargeLevel = Mathf.Clamp(_thrusterChargeLevel, 0, _thrusterChargeLevelMax);

        // Set _canUseThrusters depending on _thrusterChargeLevel
        if (_thrusterChargeLevel <= 0.0f)
        {
            _canUseThrusters = false;
        }
        else if (_thrusterChargeLevel >= (_thrusterChargeLevelMax / 0.75f))
        {
            _canUseThrusters = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canUseThrusters)
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
        _canFire = Time.time + _firerate;

        if (_ammoCount > 0)
        {
            Vector3 _laserPosition = transform.position + _laserOffset;

            if (_tripleShotActive == true && _wideShotActive == false)
            {
                Instantiate(_tripleShotlaserPrefab, transform.position, Quaternion.identity);
            }
            else if (_tripleShotActive == false && _wideShotActive == true)
            {
                FireWideShot();
            }
            else 
            {
                Instantiate(_laserPrefab, _laserPosition, Quaternion.identity);
            }

            _ammoCount--;
            _UIManager.UpdateAmmo(_ammoCount);

            _sfxAudioSource.clip = _laserShotAudioClip;
            _sfxAudioSource.Play();
        }
    }

    void FireWideShot()
    {
        Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.Euler(0, 0, 45f));
        Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.Euler(0, 0, 90f));
        Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.Euler(0, 0, -45f));
        Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.Euler(0, 0, -90f));
    }

    public void Damage()
    {
        if (!_shieldsActiveAlready)
        {
            _lives--;
            _UIManager.UpdateLives(_lives);
            UpdateSmokeDamage();

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
            // Feature: Shield Strength
            // ● Allow for 3 hits on the shield to accommodate visualization
            if (_shieldStrength >=1 )
            {
                --_shieldStrength;
            } 

            // _shieldStrength may become 0 after damage. Attaching this conditional to the one above as an else introduced an error.
            if (_shieldStrength == 0) 
            {
                _shieldsActiveAlready = false;
            }

            ShieldsUpdateVisualization();
        }
    }

    public void TripleShotActive(float _duration)
    {
        _tripleShotActive = true;
        _wideShotActive = false;
        StartCoroutine(TripleShotDuration(_duration));
    }

    IEnumerator TripleShotDuration(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _tripleShotActive = false;
    }

    public void WideShotActive(float _duration)
    {
        _tripleShotActive = false;
        _wideShotActive = true;
        StartCoroutine(WideShotDuration(_duration));
    }

    IEnumerator WideShotDuration(float _delay)
    {
        // wait 5 seconds
        yield return new WaitForSeconds(_delay);
        _wideShotActive = false;
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

    public void ShieldsActive()
    {
        //Debug.Log("ShieldsActive:Begin");
        _shieldsOnPlayer.SetActive(true);
        _shieldsActiveAlready = true;
        _shieldStrength = 3;
        ShieldsUpdateVisualization();
    }

    private void ShieldsUpdateVisualization()
    {
        //Debug.Log("ShieldsUpdateVisualization:Begin:_shieldStrength=" + _shieldStrength);

        if (_shieldStrength > 0)
        {
            _shieldsOnPlayerSpriteRenderer.color = _shieldsColor[_shieldStrength - 1];
            _shieldsOnPlayer.transform.localScale = new Vector3(_shieldsScaling[_shieldStrength - 1], _shieldsScaling[_shieldStrength - 1], 1);
        } else
        {
            _shieldsOnPlayer.SetActive(false);
        }
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

    public void AmmoRefill()
    {
        _ammoCount = 15;
        _UIManager.UpdateAmmo(_ammoCount);
    }

    private void UpdateSmokeDamage()
    {
        Debug.Log("Player::VFX_Smoke: _lives="+_lives);
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

    public void AddShip()
    {
        Debug.Log("Player::AddShip: _lives=" + _lives);
        if (_lives < 3)
        {
            _lives++;
            Debug.Log("Player::AddShip: Ship added. _lives=" + _lives);
            UpdateSmokeDamage();
            _UIManager.UpdateLives(_lives);
        }
    }

}
