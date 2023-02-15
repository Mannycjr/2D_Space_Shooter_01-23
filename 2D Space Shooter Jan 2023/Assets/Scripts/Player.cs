using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private int _lives = 3;
    private Vector3 _initPosition = new Vector3(0, -3.8f, 0);

    private float horizontalInput;
    private float verticalInput;

    [SerializeField] private GameObject _laserPrefab;
    private Vector3 _laserOffset = new Vector3(0, 0.8f, 0);
    [SerializeField] private float _firerate = 0.15f;
    private float _canFire = -1.0f;

    // Start is called before the first frame update
    void Start()
    {
        // take the current position = new position (0,0,0)
        transform.position = _initPosition;
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
        float verticalLimit = 5.5f;
        float horizontalLimit = 11.0f;

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

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
        Instantiate(_laserPrefab, _laserPosition, Quaternion.identity);
    }

    public void Damage()
    {
        _lives--;

        if (_lives < 1)
        {
            Destroy(this.gameObject);
        }
    }

}
