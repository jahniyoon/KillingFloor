using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviourPun
{

    public ParticleSystem explosion;
    public AudioSource explosionSound;
    public GameObject grenade;
    public int viewId; 
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Explosion", 3f);
    }

    public void Explosion()
    {
        grenade.SetActive(false);
        explosion.Play();
        explosionSound.Play();

        Destroy(gameObject, 5.5f);
    }
    public void setViewId(int id)
    {
        viewId = id;
    }
}
