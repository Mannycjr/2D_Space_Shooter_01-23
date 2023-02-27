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
         transform.Translate(Vector3.down * _speed * Time.deltaTime);       
        
        float _randomXPos = Random.Range(-_horizontalLimit, _horizontalLimit);

        //if bottom of screen
        if(transform.position.y <= -_verticalLimit)
        {
            //respawn at top with a new random x position
            transform.position = new Vector3(_randomXPos, _verticalLimit, 0);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // Damage the player
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Destroy(this.gameObject);
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

    }
}
