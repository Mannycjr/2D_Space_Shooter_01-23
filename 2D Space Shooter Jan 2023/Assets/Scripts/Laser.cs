using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public enum _laserIDs
    {
        Normal,
        LaserBeam
    }

    public _laserIDs _laserType;
    private int _speed = 8;
    private float _xLimit = 11.0f;
    private float _yLimit = 6.0f;
    private bool _isPlayerLaser = true;

    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;

    // Update is called once per frame
    void Update()
    {
        if (_isPlayerLaser == true)
        {
            MoveUp();
        }
        else 
        {
            if (_laserType == _laserIDs.Normal)
            { 
                MoveDown();
            }
        }

    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if ( (Mathf.Abs(transform.position.y) > _yLimit ) | (Mathf.Abs(transform.position.x) > _xLimit ) )
        {
            //check if this object is a parent

            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }        
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if ( (transform.position.y < -_yLimit) | (Mathf.Abs(transform.position.x) > _xLimit) )
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void PlayerLaser()
    {
        _isPlayerLaser = true;
    }

    public void EnemyLaser()
    {
        _isPlayerLaser = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isPlayerLaser == false)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            
            if (_laserType==_laserIDs.Normal)
            {
                ExplosionAnim(transform.position);
                Destroy(GetComponent<Collider2D>()); // Do not collide any more
                Destroy(GetComponent<SpriteRenderer>());
                Destroy(this.gameObject, 2.7f);
            } else if (_laserType == _laserIDs.LaserBeam)
            {
                ExplosionAnim(other.transform.position);
            }
        }
    }

    private void ExplosionAnim(Vector3 _explosionPosition)
    {
        _explosionInstance = Instantiate(_explosionPrefab, _explosionPosition, transform.rotation);

        Destroy(_explosionInstance, 2.7f);
    }

}
