using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Transform targetObj;             // �÷��̾� ����
    public Transform rightHandObj = null;   // ������ �׷�
    public Transform leftHandObj = null;    // �޼� �׷�

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
