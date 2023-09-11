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

    public int hitPos;

    /*
    0: Left
    1: front
    2: Right
    3: Head
    */

    public bool isCoroutine = false;
    private bool isDeath = false;

    private void Awake()
    {
        navigation = GetComponent<NormalNavigation>();
        ani = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        ZombieSetting();
        ani.runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
        aniCoroutine = StartCoroutine(AnimationCoroutine(0.0f, 1.0f, 2.0f, true, "null"));
    }
    private void Start()
    {
        ZombieSetting();
        ani.runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
        aniCoroutine = StartCoroutine(AnimationCoroutine(0.0f, 1.0f, 2.0f, true, "null"));
    }

    private void Update()
    {
        if (isDeath == false)
        {
            if (health <= 0)
            {
                StopCoroutine(aniCoroutine);
                StartCoroutine(AnimationCoroutine(0.0f, 1.0f, 2.0f, true, "null"));
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
                (health, damage, coin) = ZombieWalk();
                break;
            case 1:
                (health, damage, coin) = ZombieRun();
                break;
            case 2:
                (health, damage, coin) = ZombieSpit();
                break;
            case 3:
                (health, damage, coin) = ZombieHide();
                break;
            case 4:
                (health, damage, coin) = ZombieNoise();
                break;
        }

        GetComponent<NavMeshAgent>().speed = 0.1f;
    }

    private IEnumerator AnimationCoroutine(float startBlend, float endBlend, float duration, bool isBlend, string checkName)
    {
        isCoroutine = true;

        if (isBlend)
        {
            if (!(thisBlend == endBlend))
            {
                timeElapsed = 0.0f;

                thisBlend = ani.GetFloat("move");

                while (timeElapsed < duration)
                {
                    timeElapsed += Time.deltaTime;

                    float time = Mathf.Clamp01(timeElapsed / duration);

                    ani.SetFloat("move", Mathf.Lerp(startBlend, endBlend, time));

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
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, 0.0f, false, "atk01"));
                break;
            case 1:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, 0.0f, false, "atk02"));
                break;
        }
    }

    public void Hit(int number)
    {
        ani.SetTrigger("isChange");

        switch (number)
        {
            case 0:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, 0.0f, false, "hitHead"));
                break;
            case 1:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, 0.0f, false, "hitLeft"));
                break;
            case 2:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, 0.0f, false, "hitFront"));
                break;
            case 3:
                StartCoroutine(AnimationCoroutine(0.0f, 0.0f, 0.0f, false, "hitRight"));
                break;
        }
    }

    private void Death()
    {
        ani.SetTrigger("Dead");

        isDeath = true;
    }

    protected virtual (float, float, int) ZombieWalk()
    {
        return base.ZombieWalk(health, damage, coin);
    }

    protected virtual (float, float, int) ZombieRun()
    {
        return base.ZombieRun(health, damage, coin);
    }

    protected virtual (float, float, int) ZombieSpit()
    {
        return base.ZombieSpit(health, damage, coin);
    }

    protected virtual (float, float, int) ZombieHide()
    {
        return base.ZombieHide(health, damage, coin);
    }

    protected virtual (float, float, int) ZombieNoise()
    {
        return base.ZombieNoise(health, damage, coin);
    }
}
