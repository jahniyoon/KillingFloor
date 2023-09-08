using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class NormalZombie : NormalZombieData
{
    public List<AnimatorController> controllers = new List<AnimatorController>();

    private Coroutine aniCoroutine;

    private NormalNavigation navigation;

    private Animator ani;

    private float timeElapsed;

    // 현재 애니메이션 좌표
    private float thisBlend;

    public bool isCoroutine = false;
    private bool isDeath = false;

    private void Awake()
    {
        navigation = GetComponent<NormalNavigation>();
        ani = GetComponent<Animator>();

        // 초기 애니메이션 좌표를 Idle로 시작
        thisBlend = 0.0f;

        ani.SetFloat("move", thisBlend);
    }

    private void OnEnable()
    {
        ZombieSetting();
        ani.runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
        aniCoroutine = StartCoroutine(AnimationCoroutine(1.0f, 2.0f, true, "null"));
    }
    private void Start()
    {
        ZombieSetting();
        ani.runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
        aniCoroutine = StartCoroutine(AnimationCoroutine(1.0f, 2.0f, true, "null"));
    }

    private void Update()
    {
        if (isDeath == false)
        {
            if (healthBody <= 0 || healthHead <= 0)
            {
                StopCoroutine(aniCoroutine);
                StartCoroutine(AnimationCoroutine(1.0f, 2.0f, true, "null"));
                Death();
            }

            if (isCoroutine == false)
            {
                if (navigation.isContact == true)
                {
                    ani.SetBool("isAttack", true);

                    Attack();
                }
                else
                {
                    ani.SetBool("isAttack", false);
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

    private IEnumerator AnimationCoroutine(float blend, float duration, bool isBlend, string checkName)
    {
        isCoroutine = true;

        if (isBlend)
        {
            if (!(thisBlend == blend))
            {
                timeElapsed = 0.0f;

                thisBlend = ani.GetFloat("move");

                while (timeElapsed < duration)
                {
                    timeElapsed += Time.deltaTime;

                    float time = Mathf.Clamp01(timeElapsed / duration);

                    ani.SetFloat("move", Mathf.Lerp(thisBlend, blend, time));

                    yield return null;
                }
            }
        }
        else
        {
            ani.SetBool(checkName, true);

            float clipTime = ani.GetCurrentAnimatorClipInfo(0).Length;

            yield return new WaitForSeconds(clipTime);

            ani.SetBool(checkName, false);
        }

        isCoroutine = false;
    }

    private void Attack()
    {
        ani.SetTrigger("isChange");

        int number = Random.Range(0, 2);

        switch (number)
        {
            case 0:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, false, "atk01"));
                break;
            case 1:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, false, "atk02"));
                break;
        }
    }

    public void Hit(int number)
    {
        ani.SetTrigger("isChange");

        switch (number)
        {
            case 0:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, false, "hitHead"));
                break;
            case 1:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, false, "hitLeft"));
                break;
            case 2:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, false, "hitFront"));
                break;
            case 3:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, false, "hitRight"));
                break;
        }
    }

    private void Death()
    {
        ani.SetTrigger("Dead");

        isDeath = true;
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
