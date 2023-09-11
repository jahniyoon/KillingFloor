using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHand : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            //근접 데미지 가 들어가는 플레이어
        }
    }
}
