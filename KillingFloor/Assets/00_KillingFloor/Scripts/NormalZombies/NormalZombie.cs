using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class NormalZombie : NormalZombieData
{
    public List<AnimatorController> controllers = new List<AnimatorController>();

    private Coroutine blendTreeMove;
    private Coroutine animatorController;

    private NormalNavigation navigation;

    private Animator ani;

    private float timeElapsed;

    // 현재 애니메이션 좌표
    private float thisBlend;
    private float coolTime;
    public int hitPos;

    /*
    0: Left
    1: front
    2: Right
    3: Head
    */

    public bool isCoroutine = false;
    private bool isDeath = false;
    private bool isHit = false;
    private bool isSkill = false;

    private void Awake()
    {
        navigation = GetComponent<NormalNavigation>();
        ani = GetComponent<Animator>();
        GetComponent<NavMeshAgent>().speed = 0.1f;
        if (gameObject.name == "ZombieWalk_01" || gameObject.name == "ZombieWalk_02" ||
            gameObject.name == "ZombieWalk_03" || gameObject.name == "ZombieWalk_04" ||
            gameObject.name == "ZombieHide_01" || gameObject.name == "ZombieHide_01")
        {
            isSkill = true;
        }
        else { /*No Event*/ }
    }

    private void OnEnable()
    {
        ZombieSetting();
    }
    private void Start()
    {
        ZombieSetting();
    }

    private void Update()
    {
        if (isDeath == false)
        {
            if (health <= 0)
            {
                Death();
            }
            if (health == health / 2 && isHit == false)
            {
                isHit = true;

                Hit(hitPos);
            }
            if (isCoroutine == false)
            {
                if (navigation.isLongContact == true && isSkill == false)
                {
                    Skill();
                }
                else if (navigation.isContact == true)
                {
                    Attack();
                }
                else { /*No Event*/ }
            }
        }
    }

    private void ZombieSetting()
    {
        ani.runtimeAnimatorController = controllers[Random.Range(0, controllers.Count)];
        blendTreeMove = StartCoroutine(BlendTreeMove(0.0f, 1.0f, 2.0f));

        if (gameObject.name == "ZombieWalk_01" || gameObject.name == "ZombieWalk_02" ||
            gameObject.name == "ZombieWalk_03" || gameObject.name == "ZombieWalk_04")
        {
            int number = Random.Range(0, 2);

            switch (number)
            {
                case 0:
                    (health, damage, coin) = ZombieWalk();
                    break;
                case 1:
                    (health, damage, coin) = ZombieRun();
                    break;
            }
        }
        else if (gameObject.name == "ZombieHide_01" || gameObject.name == "ZombieHide_01")
        {
            (health, damage, coin) = ZombieHide();
        }
        else if (gameObject.name == "ZombieSpit_01" || gameObject.name == "ZombieSpit_02")
        {
            (health, damage, coin) = ZombieSpit();
        }
        else if (gameObject.name == "ZombieNoise")
        {
            (health, damage, coin) = ZombieNoise();
        }
    }

    private IEnumerator BlendTreeMove(float startBlend, float endBlend, float duration)
    {
        isCoroutine = true;

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

        isCoroutine = false;
    }

    private IEnumerator AnimatorController(string checkName)
    {
        isCoroutine = true;
        ani.SetBool(checkName, true);

        yield return new WaitForSeconds(ani.GetCurrentAnimatorStateInfo(0).length);

        ani.SetBool(checkName, false);
        isCoroutine = false;
    }

    private IEnumerator CoolTime(float time)
    {
        isSkill = true;

        coolTime = 0.0f;

        while (coolTime <= time)
        {
            coolTime += Time.deltaTime;

            yield return null;
        }

        isSkill = false;
    }

    private void Attack()
    {
        int number = Random.Range(0, 2);

        switch (number)
        {
            case 0:
                animatorController = StartCoroutine(AnimatorController("atk01"));
                break;
            case 1:
                animatorController = StartCoroutine(AnimatorController("atk02"));
                break;
        }
    }

    public void Hit(int number)
    {
        if (blendTreeMove != null) { StopCoroutine(blendTreeMove); }
        if (animatorController != null) { StopCoroutine(animatorController); }

        switch (number)
        {
            case 0: // 왼쪽
                ani.SetFloat("hitPosX", 0.0f);
                ani.SetFloat("hitPosY", -1.0f);
                break;
            case 1: // 정면
                ani.SetFloat("hitPosX", -1.0f);
                ani.SetFloat("hitPosY", 0.0f);
                break;
            case 2: // 오른쪽
                ani.SetFloat("hitPosX", 0.0f);
                ani.SetFloat("hitPosY", 1.0f);
                break;
            case 3: // 머리
                ani.SetFloat("hitPosX", 1.0f);
                ani.SetFloat("hitPosY", 0.0f);
                break;
        }

        animatorController = StartCoroutine(AnimatorController("isHit"));
    }

    public void Skill()
    {
        StartCoroutine(CoolTime(10.0f));

        animatorController = StartCoroutine(AnimatorController("atkScreaming"));
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

    //Legacy:
    //private IEnumerator AnimationCoroutine(float startBlend, float endBlend, float duration, bool isBlend, string checkName)
    //{
    //    isCoroutine = true;

    //    if (isBlend)
    //    {
    //        if (!(thisBlend == endBlend))
    //        {
    //            timeElapsed = 0.0f;

    //            thisBlend = ani.GetFloat("move");

    //            while (timeElapsed < duration)
    //            {
    //                timeElapsed += Time.deltaTime;

    //                float time = Mathf.Clamp01(timeElapsed / duration);

    //                ani.SetFloat("move", Mathf.Lerp(startBlend, endBlend, time));

    //                yield return null;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        ani.SetBool(checkName, true);

    //        float clipTime = ani.GetCurrentAnimatorClipInfo(0).Length;

    //        yield return new WaitForSeconds(clipTime);

    //        ani.SetBool(checkName, false);
    //    }

    //    isCoroutine = false;
    //}
}
