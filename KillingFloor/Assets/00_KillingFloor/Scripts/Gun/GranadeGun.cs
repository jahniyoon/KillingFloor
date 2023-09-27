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
        TriggerExplosionPlay();

    }
    private void TriggerExplosionPlay()
    {
        photonView.RPC("TriggerMasterAct", RpcTarget.MasterClient);

    }
    [PunRPC]
    private void TriggerMasterAct()
    {
        photonView.RPC("TriggerAllAct", RpcTarget.All);
    }
    [PunRPC]
    private void TriggerAllAct()
    {
        if (!explosionSound.isPlaying)
        {
            actchk = true;
            grenade.SetActive(false);
            explosion.Play();
            explosionSound.Play();
            Invoke("ActFalse", 0.3f);
        }
    }







    private void ExplosionPlay()
    {
        photonView.RPC("MasterAct", RpcTarget.MasterClient);

    }
    [PunRPC]
    private void MasterAct()
    {
        photonView.RPC("AllAct", RpcTarget.All);
    }
    [PunRPC]
    private void AllAct()
    {
        if (actchk)
        {
            grenade.SetActive(false);
            explosion.Play();
            explosionSound.Play();
            Invoke("ActFalse", 0.3f);
        }
    }

   
    private void ActFalse()
    {
        photonView.RPC("EndMasterAct", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void EndMasterAct()
    {
        photonView.RPC("EndAllAct", RpcTarget.All);
    }
    [PunRPC]
    private void EndAllAct()
    {
        explosion.Stop();
        explosionSound.Stop();
        grenade.SetActive(true);
        actchk = false;
        gameObject.SetActive(false);
    }

}
