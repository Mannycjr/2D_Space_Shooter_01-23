using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private AudioClip _sfxClipExplosion;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource.PlayClipAtPoint(_sfxClipExplosion, new Vector3(0, 0, -9));
        Destroy(this.gameObject, 2.75f);
    }
}
