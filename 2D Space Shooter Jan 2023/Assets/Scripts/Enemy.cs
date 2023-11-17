using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum _enemyIDs
    {
        Standard,
        LaserBeam,
        SmartRearLaser,
        AvoidLaser
    }

    public _enemyIDs _enemyID;
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;

    private GameManager _gameManager;
    private SpawnManager _spawnManager;

    GameObject _laserSpawnPoint;
    GameObject _laserSpawnPointBack;
    private float _verticalLimit = 7.0f;
    private float _horizontalLimit = 11.0f;

    private Player _player;

    [Header("Standard Enemy")]
    // handle to animator component
    private Animator _enemyAnimator;
    private float _explosionAnimLength = 0.0f;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _sfxClipExplosion;
    [SerializeField] private AudioClip _sfxClipLaser;

    [SerializeField] private GameObject _laserPrefab;

    private float _fireRate = 3.0f;
    private float _canFireAtTime = -1;

    private bool _isDestroyed = false;

    private bool _waveEnded = false;
    private float _randomWaitWavyTurn = -1;

    private GameObject _enemyLaserProjectile;

    [Header("LaserBeam Enemy Only")]
    private bool _LaserBeamON = false;
    public int afterLevelXLaserBeamEnemyWavyMove = 7; // When Laser Beam Enemy can move in Wavy motion, difficult for player

    [Header("Enemy Shields Only")]
    public int enemyShieldsChances = 0; // Chance for enemy to have shields. Higher number, less chance to have shields. 1 = has shields. 0 = no shields at all, Default.
    [SerializeField] private bool _enemyShieldsActiveAlready = false;
    [SerializeField] private int _enemyShieldstrength = 0; // Max = 1
    [SerializeField] private GameObject _enemyShieldsOnEnemy;
    [SerializeField] private float _enemyShieldsScaling;
    [SerializeField] private Color _enemyShieldsColor; // Double check user-set color's Alpha channel setting in Inspector.

    public int afterLevelXLaserBeamEnemyRamPlayerMove = 1; // Default = 8
    [SerializeField] private bool _aggressiveEnemy = false;
    public float _rammingDistance = 5.0f;

    private bool enemyBehindPlayer = false;

    [Header("Enemy Avoid Laser Only")]
    [SerializeField] private bool _avoidLaser = false;
    [SerializeField] private float _avoidDistance = 3.0f;

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
        if (_gameManager == null)
        {
            Debug.LogError("Enemy::Start(). Spawn Manager is NULL");
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

        _laserSpawnPoint = this.gameObject.transform.GetChild(0).gameObject; // First child object is the front spawn point
        if ((_laserSpawnPoint == null) | (this.gameObject.transform.GetChild(0).name != "Laser_Spawn_Front"))
        {
            Debug.LogError("Enemy::Start() _laserSpawnPoint is NULL or not Laser_Spawn_Front.");
        }

        switch (_enemyID)
        {
            case _enemyIDs.LaserBeam:
                _explosionAnimLength = 0.0f;
                break;
            case _enemyIDs.SmartRearLaser:
          
                _explosionAnimLength = 0.0f;
                _laserSpawnPointBack = this.gameObject.transform.GetChild(1).gameObject; // Second child object is the front back point
                if ((_laserSpawnPointBack == null) | (this.gameObject.transform.GetChild(1).name != "Laser_Spawn_Back"))
                {
                    Debug.LogError("Enemy::Start() _laserSpawnPoint is NULL or not Laser_Spawn_Back.");
                }
                break;
            case _enemyIDs.Standard:
            default:
                if (_enemyShieldsOnEnemy == null)
                {
                    Debug.LogError("Enemy::Start: No Enemy Shields Game Object");
                }
                _explosionAnimLength = 2.6f;
                Invoke("SheildsInitialize", 0.1f);
                DetermineIfPowerupInfront();
                break;
        }
    }

    private void SheildsInitialize()
    {
        if ((enemyShieldsChances > 0) && (Random.Range(1, enemyShieldsChances) == 1)) // 
        {
            ShieldsActive();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (_enemyID)
        {
            case _enemyIDs.LaserBeam:
                DetermineEnemyAggression();  // sets _aggressiveEnemy according to wave level, distance to player, chance random assignment
                if (!_aggressiveEnemy)
                {
                    if (_gameManager.waveID > afterLevelXLaserBeamEnemyWavyMove) // afterLevelXLaserBeamEnemyWavyMove = 7 (Default)
                    {
                        CalculateMovementWavy();
                    }
                    else
                    {
                        CalculateMovementStandard();
                    }
                } else if (_aggressiveEnemy) // Enemy is close enough to player to Ram and advanced Wave Level
                {
                    RamPlayer();
                }
                
                FireLaserBeam();
                break;
            case _enemyIDs.SmartRearLaser:
                CalculateMovementStandard();
                FireLaserBehind();
                break;
            case _enemyIDs.AvoidLaser:

                DetermineAvoidLaser();
                break;
            case _enemyIDs.Standard:
            default:
                CalculateMovementStandard();
                FireLaserNormal();
                break;
        }

        
        
    }

    private void CalculateMovementStandard()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        CalcMovementAtScreenLimits();
    }

    private void CalculateMovementWavy()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        
        int _randomMultiplier = 1;
        if (Time.time > _randomWaitWavyTurn)
        {
            _randomWaitWavyTurn = Time.time + Random.Range(0f, 2f);
            _randomMultiplier = Random.Range(-45,45);
        }

        float _newRotation = Mathf.Cos(Time.time* _randomMultiplier) * Time.deltaTime * 45f;
        //Debug.Log("Enemy::CalculateMovementWavy:_newRotation=" + _newRotation);
        transform.Rotate(0,0, _newRotation);
        CalcMovementAtScreenLimits();
    }

    private void CalculateMovementAvoidLaser(Laser _avoidThisLaser)
    {

        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        CalcMovementAtScreenLimits();
    }

    private void CalcMovementAtScreenLimits()
    {
        float _randomXPos = Random.Range(-_horizontalLimit, _horizontalLimit);

        //if bottom of screen
        if (transform.position.y <= -_verticalLimit)
        {
            //move to top with a new random x position
            transform.position = new Vector3(_randomXPos, _verticalLimit, 0);
            DetermineIfPowerupInfront();
        }

        // if left or right side of screen
        if (Mathf.Abs(transform.position.x) > _horizontalLimit)
        {
            float _randomYPos = Random.Range(-_verticalLimit, _verticalLimit);
            transform.position = new Vector3(-transform.position.x, _randomYPos, 0);
            DetermineIfPowerupInfront();
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

    private void FireLaserNormal()
    {
        //Debug.Log("Enemy::FireLaserNormal: Begin");

        if (Time.time > _canFireAtTime && _isDestroyed == false)
        {
            _fireRate = Random.Range(0f, 5f);
            _canFireAtTime = Time.time + _fireRate;

            GameObject _enemyLaser = Instantiate(_laserPrefab, _laserSpawnPoint.transform.position, transform.rotation);

            _enemyLaser.transform.parent = _spawnManager.enemyLaserStandardContainer.transform;

            Laser[] lasers = _enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].EnemyLaser(); // Marks it as enemy laser instead of player's laser

            }

            _audioSource.clip = _sfxClipLaser;
            _audioSource.Play(0);
        }

    }

    private void FireLaserBeam()
    {

        if ( (Time.time > _canFireAtTime) && (_isDestroyed == false) && (_LaserBeamON == false) )
        {
            float _laserBeamDuration = 2.75f; 

            _fireRate = Random.Range(10f, 20f); // Infrequent firing
            _canFireAtTime = Time.time + _fireRate;

            _audioSource.clip = _sfxClipLaser;
            _audioSource.Play(0);
            _audioSource.volume = 0.25f;
            StartCoroutine(LaserBeamOn(_laserBeamDuration));
        }
    }

    private void FireLaserBehind()
    {
        //Debug.Log("Enemy::FireLaserBehind: Begin");

        if (Time.time > _canFireAtTime && _isDestroyed == false)
        {
            //_fireRate = Random.Range(0f, 5f);
            _fireRate = 1.0f;
            _canFireAtTime = Time.time + _fireRate;

            DetermineIfBehind();

            if (enemyBehindPlayer)
            {
                GameObject _enemyLaser = Instantiate(_laserPrefab, _laserSpawnPointBack.transform.position, Quaternion.Euler(0, 0, 180f));

                Laser[] lasers = _enemyLaser.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].EnemyLaser(); // Marks it as enemy laser instead of player's laser

                }

                _audioSource.clip = _sfxClipLaser;
                _audioSource.Play(0);
            }

        }
    }

    IEnumerator LaserBeamOn(float Duration)
    {
        _LaserBeamON = true;
        _enemyLaserProjectile = Instantiate(_laserPrefab, _laserSpawnPoint.transform.position, transform.rotation);
        _enemyLaserProjectile.GetComponent<Laser>().EnemyLaser(); // Marks it as enemy laser instead of player's laser
        _enemyLaserProjectile.transform.parent = transform; //Parent LaserBeam to this enemy
        yield return new WaitForSeconds(Duration);
        LaserBeamOff();
    }

    private void LaserBeamOff()
    {
        //Debug.Log("Enemy::LaserBeamOFF: Begin and DESTROY");
        Destroy(_enemyLaserProjectile);
        _LaserBeamON = false;
    }

    private void DamageEnemy()
    {
        if (_enemyShieldsActiveAlready)
        {
            ExplosionOnlyAnim();
            ShieldsNOTActive();
        }
        else
        {
            DestoryEnemy();
        }
    }

    private void DestoryEnemy()
    {
        _isDestroyed = true;
        _audioSource.clip = _sfxClipExplosion;
        _audioSource.Play(0);

        switch (_enemyID)
        {
            case _enemyIDs.LaserBeam:
                ExplosionOnlyAnim();
                Destroy(this.GetComponentInChildren<Laser>());
                break;
            case _enemyIDs.SmartRearLaser:
                ExplosionOnlyAnim();
                break;
            case _enemyIDs.Standard:
            default:
                _enemyAnimator.SetTrigger("OnEnemyDeath"); // Explosion animaiton with Standard Enemy in beginning
                break;
        }

        Destroy(GetComponent<Collider2D>()); // Do not collide any more
        _speed = 0; // No movement after shot

        Destroy(this.gameObject, _explosionAnimLength);
        
    }

    private void ExplosionOnlyAnim()
    {
        _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(_explosionInstance, 4.0f);
    }

    public void ClearField()
    {
        _canFireAtTime = -1;
        _waveEnded = true;
    }

    public void ShieldsActive()
    {
        _enemyShieldsOnEnemy.SetActive(true);
        _enemyShieldsActiveAlready = true;
        _enemyShieldstrength = 1;
    }

    public void ShieldsNOTActive()
    {
        _enemyShieldsOnEnemy.SetActive(false);
        _enemyShieldsActiveAlready = false;
        _enemyShieldstrength = 0;
    }

    private void DetermineEnemyAggression() //sets _aggressiveEnemy according to wave level, distance to player
    {
        if ((_gameManager.waveID > afterLevelXLaserBeamEnemyRamPlayerMove) && _player != null)
        {
            float _distanceToPlayer = Vector3.Distance(this.transform.position, _player.transform.position);
            //Debug.Log("Enemy::DetermineEnemyAggression:_distance=" + _distanceToPlayer);

            if (_distanceToPlayer < _rammingDistance)
            {
                _aggressiveEnemy = true;
            }
        } else
        {
            _aggressiveEnemy = false;
        }
    }

    private void DetermineAvoidLaser()
    {
        // Debug.Log("');
        Vector2 _enemyMoveDirection = Vector2.down;
        Laser[] _trackLasers = _spawnManager.laserStandardContainer.GetComponentsInChildren<Laser>();

        for (int i = 0; i < _trackLasers.Length; i++)
        {

            Vector2 _directionToLaser = _trackLasers[i].transform.position - transform.position;

            if (_directionToLaser.magnitude < _avoidDistance)
            {
                _avoidLaser = true;
                _enemyMoveDirection = _directionToLaser;
            } else
            {
                _avoidLaser = false;
            }

        }

        transform.Translate(_enemyMoveDirection * _speed * Time.deltaTime);

        CalcMovementAtScreenLimits();
    }

    private void RamPlayer()
    {
        //Debug.Log("Enemy::RamPlayer:Begin");
        if (_player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, _speed * 1.5f * Time.deltaTime);
        }   
    }

    private void DetermineIfBehind()
    {
        if (_player != null)
        {

            Vector3 direction = _player.transform.position - transform.position;
            direction.Normalize();

            float checkBehind = Vector3.Dot(direction, -transform.up);
            //Debug.Log("Enemy::DetermineIfBehind: checkBehind=" + checkBehind+ " *****************");
            if ((checkBehind > -0.75f) & (checkBehind < 0.0f))
            {
                enemyBehindPlayer = true;
                //Debug.Log("Enemy::DetermineIfBehind: enemyBehindPlayer = true *****************");
            } else
            {
                enemyBehindPlayer = false;
            }
        }

    }

    private void DetermineIfPowerupInfront()
    {
        // Make array of powerups. Second child of the Spawn_Manager will always be the "Powerup_Container" game object
        //Debug.Log("Enemy::Start:This enemy's parent is " + transform.parent.parent.name);
        Powerup[] _allPowerUps = transform.parent.parent.GetChild(1).GetComponentsInChildren<Powerup>();
        //Debug.Log("Enemy::Start:_allPowerUps.Length= " + _allPowerUps.Length);
        for (int i = 0; i < _allPowerUps.Length; i++)
        {
            Vector3 directionPowerup = _allPowerUps[i].transform.position - transform.position;
            directionPowerup.Normalize();

            // Check if the powerup is in front of this enemy
            float checkFront = Vector3.Dot(directionPowerup, -transform.up);
            if ((checkFront > -0.75f) & (checkFront < 0.0f))
            {
                Invoke("FireLaserNormal", 0.1f);
            }
        }
    }
}
