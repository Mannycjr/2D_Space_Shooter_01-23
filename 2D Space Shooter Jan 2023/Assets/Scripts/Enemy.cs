using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    private float _verticalLimit = 7.0f;
    private float _horizontalLimit = 11.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float _randomXPos = Random.Range(-_horizontalLimit, _horizontalLimit);

        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //if bottom of screen
        if(transform.position.y <= -_verticalLimit)
        {
            //respawn at top with a new random x position
            transform.position = new Vector3(_randomXPos, _verticalLimit, 0);
        }
        
    }
}
