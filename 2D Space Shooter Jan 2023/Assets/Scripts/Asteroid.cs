using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private float _rotationSpeed = 20.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private GameObject _explosionInstance;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _explosionInstance = Instantiate(_explosionPrefab, transform.position, transform.rotation);
            Destroy(_explosionInstance,2.5f); 
    
        }

        Destroy(other.gameObject);
        
        Destroy(this.gameObject);
    }
}
