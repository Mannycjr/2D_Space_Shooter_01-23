using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    private float _verticalLimit = 7.0f;
    private float _horizontalLimit = 11.0f;

    private Player _player;
    // handle to animator component
    private Animator _enemyAnimator;
    private float _explosionAnimLength = 2.6f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Enemy::Start() No _player");
        }

        _enemyAnimator = GetComponent<Animator>();
        if (_enemyAnimator == null)
        {
            Debug.LogError("Enemy::Start() No _enemyAnimator");
        } else
        {
            Debug.Log("Enemy::Start() _enemyAnimator Exists");
        }

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
            // trigger anim
            DestoryEnemy();
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.PlayerScoreUpdate(10);
            }
            DestoryEnemy();
        }

    }

    private void DestoryEnemy()
    {
        Debug.Log("Enemy:DestoryEnemy() Begin");
        _enemyAnimator.SetTrigger("OnEnemyDeath");
        Debug.Log("Enemy:DestoryEnemy() played enemy explosion animation");
        Destroy(GetComponent<Collider2D>()); // Do not collide any more
        _speed = 0; // No movement after shot

        Destroy(this.gameObject, _explosionAnimLength);
    }
}
