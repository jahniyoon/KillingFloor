using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public float maxHealth; // 최대 체력
    public float curHealth { get; protected set; } // 현재 체력
    public bool isDead { get; protected set; }  // 생존 여부

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
