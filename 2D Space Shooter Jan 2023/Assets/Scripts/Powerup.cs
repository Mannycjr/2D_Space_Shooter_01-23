using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private float _powerupDuration = 7.0f;
    public enum _powerupIDs
    {
        TripleShot,
        SpeedBoost,
        Shields
    }
    [SerializeField] private _powerupIDs _powerupID;
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
                switch (_powerupID)
                {
                    case _powerupIDs.TripleShot:
                        player.TripleShotActive();
                        Debug.Log("Powerup::OnTriggerEnter2D:switch _powerupID=0 TripleShot");
                        break;
                    case _powerupIDs.SpeedBoost:
                        player.SpeedBoostActive();
                        Debug.Log("Powerup::OnTriggerEnter2D:switch _powerupID=1 SpeedBoost");
                        break;
                    case _powerupIDs.Shields:
                        Debug.Log("Powerup::OnTriggerEnter2D:switch _powerupID=2 Shields");
                        break;
                    default:
                        Debug.Log("Powerup::OnTriggerEnter2D:switch No_powerupID");
                        break;
                }
                
            }
            else
            {
                Debug.LogError("Powerup::OnTriggerEnter2D: player is null");
            }


            Destroy(this.gameObject);
        }
    }

}
