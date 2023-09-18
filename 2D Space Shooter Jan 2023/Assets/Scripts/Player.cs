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
    [SerializeField] private int _ammoCount = 20;
    [SerializeField] private int _ammoMax = 20;

    SpawnManager _spawnManager;

    [SerializeField] private float _speedBoostMultiplierShift = 2.5f;

    //Powerups variables
    [SerializeField] private bool _tripleShotActive = false;
    [SerializeField] private bool _wideShotActive = false;
    [SerializeField] private bool _speedBoostPowerupActive = false;

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

    [Header("Shift Key Thrusters UI Bar")]
    [SerializeField] private float _shiftKeyThrustersWaitTimeLimit = 3.0f;
    [SerializeField] private float _thrusterChargeLevelMax = 10.0f;
    [SerializeField] private float _thrusterChargeLevel;
    [SerializeField] private float _changeDecreaseThrusterChargeBy = 1.5f;
    [SerializeField] private float _changeIncreaseThrusterChargeBy = 0.001f;
    [SerializeField] private bool _canUseThrusters = true;
    [SerializeField] private bool _speedBoostShiftActive = false; // Opposite of _speedBoostPowerupActive

    private MainCamera _mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _initPosition;
        _thrusterChargeLevel = _thrusterChargeLevelMax;
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _mainCamera = GameObject.Find("Main_Camera").GetComponent<MainCamera>();

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
            _UIManager.UpdateAmmo(_ammoCount, _ammoMax);
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

        if (_mainCamera == null)
        {
            Debug.LogError("Player::Start: _mainCamera is NULL");
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
        else if (_thrusterChargeLevel >= 0.0f)
        {
            _canUseThrusters = true;
        }

        //Debug.Log("Player::Update:_thrusterChargeLevel=" + _thrusterChargeLevel + " _canUseThrusters="+ _canUseThrusters);

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canUseThrusters)
        {
            //Debug.Log("Player::Update: GetKeyDown(KeyCode.LeftShift) _canUseThrusters=True. pre-SpeedBoostActiveShift()");
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
        //Debug.Log("Player::CalculateMovement:_speedBoostShiftActive=" + _speedBoostShiftActive);

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

        if (_speedBoostShiftActive)
        {
            ThrustersActive();
        }
        else if (!_speedBoostShiftActive)
        {
            StartCoroutine(ThrustersPowerReplenishRoutine());
        }

    }

    // Super Speed Thrusters are ON
    void ThrustersActive()
    {
        Debug.Log("Player::ThrustersActive:_canUseThrusters=" + _canUseThrusters);

        if (_canUseThrusters == true)
        {
            _thrusterChargeLevel -= Time.deltaTime * _changeDecreaseThrusterChargeBy;
            _UIManager.UpdateThrustersSlider(_thrusterChargeLevel); //Change thruster bar UI: reduce 
            //Debug.Log("_thrusterChargeLevel=" + _thrusterChargeLevel);

            if (_thrusterChargeLevel <= (_thrusterChargeLevelMax * 0.25))
            {
                _UIManager.ThurstersSliderUsableColor(false);

                if (_thrusterChargeLevel <= 0)
                {
                    _speedBoostShiftActive = false;
                    _canUseThrusters = false;
                    SpeedReset();
                }

            }
            else
            {
                _UIManager.ThurstersSliderUsableColor(true);
            }
        }
    }

    // Shift Key Thrusters Cool Down System. Thrusters NOT Active
    IEnumerator ThrustersPowerReplenishRoutine()
    {
        //Debug.Log("Player::ThrustersPowerReplenishRoutine:Begin");

        yield return new WaitForSeconds(_shiftKeyThrustersWaitTimeLimit);

        while ((_thrusterChargeLevel <= _thrusterChargeLevelMax) && !_speedBoostShiftActive)
        {
            _thrusterChargeLevel += Time.deltaTime * _changeIncreaseThrusterChargeBy;
            _UIManager.UpdateThrustersSlider(_thrusterChargeLevel); // Change thruster bar UI: increase

            if (_thrusterChargeLevel >= (_thrusterChargeLevelMax * 0.25))
            {
                _UIManager.ThurstersSliderUsableColor(true);
            }
            yield return new WaitForEndOfFrame();
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
            _UIManager.UpdateAmmo(_ammoCount, _ammoMax);

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
            _mainCamera.CameraShake();

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
        _ammoCount = 20;
        _UIManager.UpdateAmmo(_ammoCount, _ammoMax);
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

    public void NoAmmo()
    {
        _ammoCount = 0;
        _UIManager.UpdateAmmo(_ammoCount, _ammoMax);
    }
}
