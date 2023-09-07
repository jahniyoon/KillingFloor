using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombieData : MonoBehaviour
{
    public float healthBody;
    public float healthHead;
    public float damage;
    public float speed;
    public int coin;

    public virtual (float, float, float, float, int) ZombieWalk(float _healthBody, float _healthHead, float _damage, float _speed, int _coin)
    {
        _healthBody = ((GameManager.instance.round * 10.0f) + (GameManager.instance.player * 10.0f)) * 2.0f;
        _healthHead = _healthBody / 2.0f;
        _damage = GameManager.instance.round + GameManager.instance.player;
        _speed = 3.0f;
        _coin = 50;

        return (_healthBody, _healthHead, _damage, _speed, _coin);
    }

    public virtual (float, float, float, float, int) ZombieRun(float _healthBody, float _healthHead, float _damage, float _speed, int _coin)
    {
        _healthBody = ((GameManager.instance.round * 10.0f) + (GameManager.instance.player * 10.0f)) * 1.5f;
        _healthHead = _healthBody / 2.0f;
        _damage = (GameManager.instance.round + GameManager.instance.player) * 1.5f;
        _speed = 5.0f;
        _coin = 70;

        return (_healthBody, _healthHead, _damage, _speed, _coin);
    }

    public virtual (float, float, float, float, int) ZombieSpit(float _healthBody, float _healthHead, float _damage, float _speed, int _coin)
    {
        _healthBody = ((GameManager.instance.round * 10.0f) + (GameManager.instance.player * 10.0f)) * 3.0f;
        _healthHead = _healthBody / 2.0f;
        _damage = GameManager.instance.round + GameManager.instance.player;
        _speed = 2.0f;
        _coin = 100;

        return (_healthBody, _healthHead, _damage, _speed, _coin);
    }

    public virtual (float, float, float, float, int) ZombieHide(float _healthBody, float _healthHead, float _damage, float _speed, int _coin)
    {
        _healthBody = ((GameManager.instance.round * 10.0f) + (GameManager.instance.player * 10.0f)) * 1.5f;
        _healthHead = _healthBody / 2;
        _damage = GameManager.instance.round + GameManager.instance.player;
        _speed = 4.0f;
        _coin = 100;

        return (_healthBody, _healthHead, _damage, _speed, _coin);
    }

    public virtual (float, float, float, float, int) ZombieNoise(float _healthBody, float _healthHead, float _damage, float _speed, int _coin)
    {
        _healthBody = ((GameManager.instance.round * 10.0f) + (GameManager.instance.player * 10.0f)) * 2.0f;
        _healthHead = _healthBody / 2.0f;
        _damage = GameManager.instance.round + GameManager.instance.player;
        _speed = 1.0f;
        _coin = 100;

        return (_healthBody, _healthHead, _damage, _speed, _coin);
    }
}
