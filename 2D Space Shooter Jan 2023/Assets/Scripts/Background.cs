using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float _speed = 2.0f;
    public float _bottomLimitY = -16.01f;
    public float _topRespawnY = 24.86f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < _bottomLimitY)
        {
            transform.position = new Vector3(transform.position.x,_topRespawnY,transform.position.z);
        }
    }
}
