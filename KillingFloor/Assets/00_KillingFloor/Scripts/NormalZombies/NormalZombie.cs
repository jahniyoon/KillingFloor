using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class NormalZombie : NormalZombieData
{
    private Animator ani;

    private float timeElapsed;

    public bool isCoroutine = false;

    private void OnEnable()
    {
        ZombieSetting();
        StartMove();
    }
    private void Start()
    {
        ZombieSetting();
        StartMove();
    }

    private void Update()
    {
        if (healthBody <= 0 || healthHead <= 0)
        {
            Death();
        }
    }

    private void ZombieSetting()
    {
        int number = Random.Range(0, 1);

        switch (number)
        {
            case 0:
                (healthBody, healthHead, damage, speed, coin) = ZombieWalk();
                break;
            case 1:
                (healthBody, healthHead, damage, speed, coin) = ZombieRun();
                break;
            case 2:
                (healthBody, healthHead, damage, speed, coin) = ZombieSpit();
                break;
            case 3:
                (healthBody, healthHead, damage, speed, coin) = ZombieHide();
                break;
            case 4:
                (healthBody, healthHead, damage, speed, coin) = ZombieNoise();
                break;
        }

        GetComponent<NavMeshAgent>().speed = speed;

        if (!gameObject.GetComponent<CapsuleCollider>())
        {
            CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0.0f, 1.0f, 0.0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 2.0f;
        }
        else { /*No Event*/ }

        if (!gameObject.GetComponent<SphereCollider>())
        {
            if (number == 2 || number == 3 || number == 4)
            {
                SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
                sphereCollider.center = new Vector3(0.0f, 1.6f, 0.0f);
                sphereCollider.radius = 7.0f;
            }
        }
        else
        {
            if (number == 0 || number == 1)
            {
                gameObject.GetComponent<SphereCollider>().enabled = false;
            }
        }
    }

    //private void OnTriggerEnter(CapsuleCollider _capsuleCollider, SphereCollider _sphereCollider)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Attack();
    //    }


    //}
    private IEnumerator StartMove()
    {
        timeElapsed = 0.0f;

        while (timeElapsed < 1.0f)
        {
            timeElapsed = Time.deltaTime;

            float time = Mathf.Clamp01(timeElapsed / 1.0f);

            ani.SetFloat("Blend", Mathf.Lerp(0.0f, 1.0f, time));

            yield return null;
        }
    }

    public IEnumerator Attack()
    {
        isCoroutine = true;

        //애니메이션 현재 진행 상황 확인
        //AnimatorStateInfo stateInfo = ani.GetCurrentAnimatorStateInfo(0);
        
        //ani.GetCurrentAnimatorStateInfo
        yield return new WaitForSeconds(1.0f);
        isCoroutine = false;
    }

    private void Death()
    {
        ani.SetTrigger("isDie");
    }

    protected virtual (float, float, float, float, int) ZombieWalk()
    {
        return base.ZombieWalk(healthBody, healthHead, damage, speed, coin);
    }

    protected virtual (float, float, float, float, int) ZombieRun()
    {
        return base.ZombieRun(healthBody, healthHead, damage, speed, coin);
    }

    protected virtual (float, float, float, float, int) ZombieSpit()
    {
        return base.ZombieSpit(healthBody, healthHead, damage, speed, coin);
    }

    protected virtual (float, float, float, float, int) ZombieHide()
    {
        return base.ZombieHide(healthBody, healthHead, damage, speed, coin);
    }

    protected virtual (float, float, float, float, int) ZombieNoise()
    {
        return base.ZombieNoise(healthBody, healthHead, damage, speed, coin);
    }
}
