using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BossSpScripts : MonoBehaviourPunCallbacks
{
    public GameObject bossPf;
    private void Awake()
    {
         PhotonNetwork.Instantiate(bossPf.name, transform.position, Quaternion.identity);
        
       
    }

}
