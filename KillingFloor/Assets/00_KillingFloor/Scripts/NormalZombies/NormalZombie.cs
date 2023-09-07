using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class NormalZombie : NormalZombieData
{
    private Coroutine aniCoroutine;

    private NormalNavigation navigation;

    private Animator ani;

    private float timeElapsed;

    // 현재 애니메이션 좌표
    private float thisPosX;
    private float thisPosY;

    public bool isCoroutine = false;

    private void Awake()
    {
        navigation = GetComponent<NormalNavigation>();
        ani = GetComponent<Animator>();

        // 초기 애니메이션 좌표를 idle로 시작
        thisPosX = 0.0f;
        thisPosY = -1.0f;

        ani.SetFloat("blendPosX", thisPosX);
        ani.SetFloat("blendPosY", thisPosY);
    }

    private void OnEnable()
    {
        ZombieSetting();
        aniCoroutine = StartCoroutine(AnimationCoroutine(0.0f, 0.0f, 2.0f, true));
    }
    private void Start()
    {
        ZombieSetting();
        aniCoroutine = StartCoroutine(AnimationCoroutine(0.0f, 0.0f, 2.0f, true));
    }

    private void Update()
    {
        if (healthBody <= 0 || healthHead <= 0)
        {
            StopCoroutine(aniCoroutine);
            StartCoroutine(AnimationCoroutine(0.0f, -1.0f, 0.0f, false));
            Death();
        }

        if (isCoroutine == false)
        {
            if (navigation.isContact == true)
            {
                Attack();
            }
            else
            {
                if (ani.GetFloat("blendPosX") != 0.0f && ani.GetFloat("blendPosY") != 0.0f)
                {
                    StartCoroutine(AnimationCoroutine(0.0f, 0.0f, 1.0f, true));
                }
            }
        }
    }

    private void ZombieSetting()
    {
        int number = Random.Range(0, 5);

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

        GetComponent<NavMeshAgent>().speed = 0.1f;

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

    private IEnumerator AnimationCoroutine(float posX, float posY, float duration, bool isCheck)
    {
        isCoroutine = true;

        if (!(thisPosX == posX && thisPosY == posY))
        {
            if (isCheck == true)
            {
                timeElapsed = 0.0f;

                thisPosX = ani.GetFloat("blendPosX");
                thisPosY = ani.GetFloat("blendPosY");

                while (timeElapsed < duration)
                {
                    timeElapsed += Time.deltaTime;

                    float time = Mathf.Clamp01(timeElapsed / duration);

                    ani.SetFloat("blendPosX", Mathf.Lerp(thisPosX, posX, time));
                    ani.SetFloat("blendPosY", Mathf.Lerp(thisPosY, posY, time));

                    yield return null;
                }
            }
            else
            {
                ani.SetFloat("blendPosX", posX);
                ani.SetFloat("blendPosY", posY);
            }
        }
        else { /*No Event*/ }

        isCoroutine = false;
    }

    private void Attack()
    {
        isCoroutine = true;

        int number = Random.Range(0, 1);

        switch (number)
        {
            case 0:
                StartCoroutine(AnimationCoroutine(-0.5f, 1.0f, 1.0f, true));
                break;
            case 1:
                StartCoroutine(AnimationCoroutine(0.0f, 1.0f, 1.0f, true));
                break;
            case 2:
                StartCoroutine(AnimationCoroutine(0.5f, 1.0f, 1.0f, true));
                break;
        }

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
