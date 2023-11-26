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
    private bool _isPlayerMissile = true;

    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;

    private SpawnManager _spawnManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManagerScript = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManagerScript == null)
        {
            Debug.LogError("Missile::Start:No _spawnManager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlayerMissile == true)
        {
            MoveUp();
        }
        else
        {
            if (MissileType == missileIDs.Normal)
            {
                //MoveDown();
            }
        }
    }

    void MoveUp()
    {
        switch (MissileType)
        {
            case missileIDs.Homing:
                
                // MoveHomingMissle();
                // moveTowards nearest enemy;
                break;
            case missileIDs.Normal:
            default:
                transform.Translate(Vector3.up * _speed * Time.deltaTime);
                break;
        }

        DestroyAtScreenLimits();

    }

    void MoveHomingMissle()
    {

    }

    void DestroyAtScreenLimits()
    {
        if ((Mathf.Abs(transform.position.y) > _yLimit) | (Mathf.Abs(transform.position.x) > _xLimit))
        {
            Destroy(this.gameObject);
        }
    }
}
