using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private int _speed = 8;
    private float _yLimit = 6.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > _yLimit )
        {
            Destroy(this.gameObject);
        }
    }
}
