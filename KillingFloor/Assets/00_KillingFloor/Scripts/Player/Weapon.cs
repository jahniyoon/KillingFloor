using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Transform targetObj;             // ÇÃ·¹ÀÌ¾î ½ÃÁ¡
    public Transform rightHandObj = null;   // ¿À¸¥¼Õ ±×·¦
    public Transform leftHandObj = null;    // ¿Þ¼Õ ±×·¦

    public int weaponID;

    float damage;
    float fireRate;
    float ammo;
    float clip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
