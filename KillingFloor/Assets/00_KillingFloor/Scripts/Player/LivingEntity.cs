using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public float maxHealth; // �ִ� ü��
    public float curHealth { get; protected set; } // ���� ü��
    public bool isDead { get; protected set; }  // ���� ����

    public void ApplayUpdatedHealth(float _curHealth, bool _isDead)
    { }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
