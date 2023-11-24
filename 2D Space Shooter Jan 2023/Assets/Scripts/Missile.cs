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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlayerMissile == true)
        {
            //MoveUp();
        }
        else
        {
            if (MissileType == missileIDs.Normal)
            {
                //MoveDown();
            }
        }
    }
}
