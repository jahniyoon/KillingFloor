using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeGun : MonoBehaviourPun
{
    public ParticleSystem explosion;
    public AudioSource explosionSound;
    public GameObject grenade;
    public int viewId;
    private bool actchk = false;

    // Start is called before the first frame update
  
  

    private void Update()
    {
        if(actchk == false)
        {
            actchk = true;
            Invoke("ExplosionPlay", 3);
            
        }
       
    }

    public void setViewId(int id)
    {
        viewId = id;
    }
    
   
    private void OnTriggerEnter(Collider other)
    {
       Debug.Log(other.transform.name);
        grenade.SetActive(false);
        explosion.Play();
        explosionSound.Play();
        Invoke("ActFalse", 0.3f);
    }
    private void ExplosionPlay()
    {
        if(actchk)
        {
            grenade.SetActive(false);
            explosion.Play();
            explosionSound.Play();
            Invoke("ActFalse", 0.3f);
        }
     
    }
    private void ActFalse()
    {
        explosion.Stop();
        explosionSound.Stop();
        grenade.SetActive(true); 
        actchk = false;
        gameObject.SetActive(false);
    }
}
