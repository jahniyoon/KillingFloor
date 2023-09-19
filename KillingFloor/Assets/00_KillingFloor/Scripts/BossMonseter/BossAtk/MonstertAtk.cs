using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstertAtk : MonoBehaviour
{
    private bool atkChk = false;
    private float damage = 8f;
    private float atkTime = 0.5f;
    private void OnTriggerStay(Collider other)
    {

        if (gameObject.name.Equals("Effect_07_Sphere"))
        {
            damage = 1;
            atkTime = 0.1f;
        }
        else
        {
            damage = 8;
            atkTime = 0.5f;
        }

        if(atkChk == false)
        {
            // 상대방으로부터 LivingEntity 타입을 가져오기 시도
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            if (attackTarget != null)
            {                // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;

                // 공격 실행
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
            Invoke("DamTime", atkTime);
            atkChk = true;
        }
      
    }

    private void DamTime()
    {
        atkChk = false;
    }
}
