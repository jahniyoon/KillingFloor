using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpne : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.wave >= GameManager.instance.MaxWave)
        {
            gameObject.transform.rotation = Quaternion.Euler(transform.rotation.x, -90f, transform.rotation.z);
        }
    }

    
}
