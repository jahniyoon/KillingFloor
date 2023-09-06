using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;

public class NormalZombie : NormalZombieData
{
    private Animator ani;

    private void Start()
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

        if (number == 0 || number == 1 || number == 3)
        {
            CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0.0f, 1.0f, 0.0f);
            collider.radius = 0.5f;
            collider.height = 2.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack();
        }


    }

    private void Attack()
    {

    }

    private void Death()
    {

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
