using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private Vector3 _initPosition = new Vector3(0, -3.8f, 0);

    public float horizontalInput;
    public float verticalInput;

    [SerializeField] private GameObject _laserPrefab;

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

        //if i hit space key
        //spawn space key

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Spacebar pressed down");
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);
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


}
