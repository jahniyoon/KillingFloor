using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackHand : MonoBehaviour
{
    public Animator parentObjectAni;

    private void Update()
    {
        if (parentObjectAni.GetCurrentAnimatorStateInfo(0).IsName("Atk01") == true ||
            parentObjectAni.GetCurrentAnimatorStateInfo(0).IsName("Atk02") == true)
        {
            GetComponent<SphereCollider>().enabled = true;
        }
        else
        {
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        { return; }
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().OnDamage(parentObjectAni.gameObject.GetComponent<NormalZombie>().damage, Vector3.zero, Vector3.zero);
        }
    }
}
