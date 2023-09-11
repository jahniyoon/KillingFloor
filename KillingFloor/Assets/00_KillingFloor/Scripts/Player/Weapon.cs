using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Transform targetObj;             // �÷��̾� ����
    public Transform rightHandObj = null;   // ������ �׷�
    public Transform leftHandObj = null;    // �޼� �׷�

    [Header("Weapon")]
    public int weaponID;
    public float damage;       // ������
    public float fireRate;     // ����ӵ� : RPM���� ���
    public float reloadRate;   // ������ �ӵ� : �ʷ� ���
    public float ammo;         // ���� źâ
    public float totalAmmo;    // �ܿ� ź��
    public float magazineSize; // źâ �뷮

}
