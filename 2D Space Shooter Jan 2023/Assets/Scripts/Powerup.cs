using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    private int _speed = 4;
    private float _verticalLimit = 7.0f;
    private float _horizontalLimit = 11.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        float _randomXPos = Random.Range(-_horizontalLimit, _horizontalLimit);

        if (transform.position.y <= -_verticalLimit)
        {
            //respawn at top with a new random x position
            transform.position = new Vector3(_randomXPos, _verticalLimit, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

    }

}
