using Photon.Pun.Demo.SlotRacer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueParticleDam : MonoBehaviour
{
    private bool atkChk = false;
    private float damage = 5f;
    private float coolTime = 0.05f;
    private ParticleSystem particleSystem;
 

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

    }
    private void OnParticleCollision(GameObject other)
    {
        if (atkChk == false)
        {
            Debug.Log(transform.name);
            if(transform.name.Equals("Effect_38_SmokeField_2") || transform.name.Equals("Effect_38_ParticleField"))
            {
                coolTime = 0;
                damage = 2000;      
            }
            else
            {
                coolTime = 0.05f;
                damage = 5;
            }
            var main = particleSystem.main;
            main.gravityModifier = 1f;


            // 상대방으로부터 LivingEntity 타입을 가져오기 시도
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            if (attackTarget != null) 
            {                // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitPoint = other.transform.position;
                Vector3 hitNormal = transform.position - other.transform.position;

                // 공격 실행
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
            Invoke("DamTime", coolTime);
            atkChk = true;
        }
    }

    private void DamTime()
    {
        atkChk = false;
    }
}
