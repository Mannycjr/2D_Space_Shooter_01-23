using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public enum missileIDs
    {
        Normal,
        Homing
    }

    public missileIDs MissileType;
    private int _speed = 5;
    private float _xLimit = 11.0f;
    private float _yLimit = 6.0f;
    
    // In the beginning of dev, missile is player only
    //private bool _isPlayerMissile = true;

    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;

    private SpawnManager _spawnManagerScript;

    float _nearestEnemyDistance = 5.0f;
    [SerializeField] private Enemy _nearestEnemy;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManagerScript = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManagerScript == null)
        {
            Debug.LogError("Missile::Start:No _spawnManager");
        }

        Enemy[] _allEnemies = _spawnManagerScript._enemyContainer.GetComponentsInChildren<Enemy>(); ;

        StartCoroutine(GetNearestEnemy(_allEnemies));
    }

    // Update is called once per frame
    void Update()
    {
        switch (MissileType)
        {
            case missileIDs.Homing:

                MoveHomingMissle();
                // moveTowards nearest enemy;
                break;
            case missileIDs.Normal:
            default:
                MoveUp();
                break;
        }

    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        DestroyAtScreenLimits();

    }

    void MoveHomingMissle()
    {
        // select closest enemy. sorting algorithm

        // move towards closest enemy

    }

    void DestroyAtScreenLimits()
    {
        if ((Mathf.Abs(transform.position.y) > _yLimit) | (Mathf.Abs(transform.position.x) > _xLimit))
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator GetNearestEnemy(Enemy[] _allEnemies)
    {
        Vector2 _distanceToEnemy = _allEnemies[0].transform.position - transform.position;
        float _previousDistance = Mathf.Abs(_distanceToEnemy.magnitude);
        _nearestEnemy = _allEnemies[0];

        for (int i = 1; i < _allEnemies.Length; i++)
        {
            _distanceToEnemy = _allEnemies[i].transform.position - transform.position;
            if ((Mathf.Abs(_distanceToEnemy.magnitude) < _nearestEnemyDistance) & (Mathf.Abs(_distanceToEnemy.magnitude) < _previousDistance))
            {
                _nearestEnemy = _allEnemies[i];
            }

        }
        return null;

    }

}
