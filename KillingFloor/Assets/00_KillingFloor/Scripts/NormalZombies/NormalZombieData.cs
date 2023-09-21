using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombieData : MonoBehaviourPun
{
    public float health;
    public float damage;
    public int coin;

    public virtual (float, float, int) ZombieWalk(float _health, float _damage, int _coin)
    {
        _health = ((GameManager.instance.round * 10.0f) +
            (GameManager.instance.player * 10.0f) +
            (GameManager.instance.difficulty * 10.0f)) * 2.0f;
        _damage = GameManager.instance.round +
            GameManager.instance.player +
            GameManager.instance.difficulty;
        _coin = 50;

        return (_health, _damage, _coin);
    }

    public virtual (float, float, int) ZombieRun(float _health, float _damage, int _coin)
    {
        _health = ((GameManager.instance.round * 10.0f) +
            (GameManager.instance.player * 10.0f) +
            (GameManager.instance.difficulty * 10.0f)) * 1.5f;
        _damage = GameManager.instance.round +
            GameManager.instance.player +
            GameManager.instance.difficulty;
        _coin = 70;

        return (_health, _damage, _coin);
    }

    public virtual (float, float, int) ZombieSpit(float _health, float _damage, int _coin)
    {
        _health = ((GameManager.instance.round * 10.0f) +
            (GameManager.instance.player * 10.0f) +
            (GameManager.instance.difficulty * 10.0f)) * 3.0f;
        _damage = GameManager.instance.round +
            GameManager.instance.player +
            GameManager.instance.difficulty;
        _coin = 100;

        return (_health, _damage, _coin);
    }

    public virtual (float, float, int) ZombieHide(float _health, float _damage, int _coin)
    {
        _health = ((GameManager.instance.round * 10.0f) +
            (GameManager.instance.player * 10.0f) +
            (GameManager.instance.difficulty * 10.0f)) * 1.5f;
        _damage = GameManager.instance.round +
            GameManager.instance.player +
            GameManager.instance.difficulty;
        _coin = 100;

        return (_health, _damage, _coin);
    }

    public virtual (float, float, int) ZombieNoise(float _health, float _damage, int _coin)
    {
        _health = ((GameManager.instance.round * 10.0f) +
            (GameManager.instance.player * 10.0f) +
            (GameManager.instance.difficulty * 10.0f)) * 2.0f;
        _damage = GameManager.instance.round +
            GameManager.instance.player + 
            GameManager.instance.difficulty;
        _coin = 100;

        return (_health, _damage, _coin);
    }
}
