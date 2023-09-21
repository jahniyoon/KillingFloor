using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    public GameObject parentObject;

    public void Hit(float damage)
    {
        parentObject.GetComponent<NormalZombie>().Hit(damage);

        if (gameObject.layer == 8)
        {
            parentObject.GetComponent<NormalZombie>().hitPos = 0;
        }
        else if (gameObject.layer == 9)
        {
            parentObject.GetComponent<NormalZombie>().hitPos = 1;
        }
        else if (gameObject.layer == 10)
        {
            parentObject.GetComponent<NormalZombie>().hitPos = 2;
        }
        else if (gameObject.layer == 11)
        {
            parentObject.GetComponent<NormalZombie>().hitPos = 3;
        }
        else { /*No Event*/ }
    }
}
