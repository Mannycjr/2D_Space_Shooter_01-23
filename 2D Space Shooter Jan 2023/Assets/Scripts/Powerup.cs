using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    private float _verticalLimit = 7.0f;
    private float _horizontalLimit = 11.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // move down
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // when leave screen, destroy object
        if (transform.position.y <= -_verticalLimit)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // Enable the tripleshot lasers in player
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.TripleShotActive();   
            }
            else
            {
                Debug.LogError("Powerup::OnTriggerEnter2D: player is null");
            }


            Destroy(this.gameObject);
        }
    }

}
