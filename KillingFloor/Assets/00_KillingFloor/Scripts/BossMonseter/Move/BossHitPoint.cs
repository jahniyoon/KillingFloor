using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitPoint : MonoBehaviour
{

    public GameObject parentObject;

    public void Hit(float damage)
    {
        transform.GetComponent<BossController>().bossHp -= damage;

     
    }
}
