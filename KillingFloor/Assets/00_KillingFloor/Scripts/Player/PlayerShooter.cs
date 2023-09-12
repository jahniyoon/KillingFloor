using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;
using static UnityEditor.Progress;

public class PlayerShooter : MonoBehaviour
{
    public enum Type { Pistol, Rifle, Melee, Heal };
    private PlayerInputs input;
    private PlayerMovement playerMovement;
    private CameraSetup cameraSet;
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11);    // ������ ���� ������ ���̾� ����ũ


    public Transform aimTarget;
    public float damage;
    public float range = 100f;
    public float reloadRate;
    public float fireRate;
    public bool isReloading;

    [Header("Animator IK")]
    protected Animator animator;
    public Animator handAnimator;

    public bool ikActive = false;
    [Range(0,1)]
    public float handIKAmount = 1;
    [Range(0, 1)]
    public float elbowIKAmount = 1;
    public Transform weaponPosition = null;    // ���� ��ġ ������
    public Transform targetObj;                // �÷��̾� ����

    [Header("TPS Weapon")]
    public Weapon equipedWeapon;
    [Range(0, 5)]
    public int weaponSlot;
    public Type weaponType;
    public Transform rightHandObj = null;   // ������
    public Transform leftHandObj = null;    // �޼�
    public Transform rightElbowObj = null;   // ������ �׷�
    public Transform leftElbowObj = null;    // �޼� �׷�

    Weapon tpsPistol;    // ������ ���� ���� ����
    Weapon tpsRifle;     // ������ ������ ���� ����

    [Header("FPS Weapon")]
    public Transform fpsPosition;
    public Transform fpsPistolObj;
    public Transform fpsRifleObj;
    public Transform fpsWeapon;



    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInputs>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraSet = GetComponent<CameraSetup>();
        animator = GetComponent<Animator>();

        // TPS ���� ��������
        tpsPistol = weaponPosition.GetChild(0).GetComponent<Weapon>();
        tpsRifle = weaponPosition.GetChild(1).GetComponent<Weapon>();
        tpsRifle.gameObject.SetActive(false);    // �̸� ���α�

        // FPS ���� ��������
        fpsPosition = transform.GetChild(0).GetChild(0).GetComponent<Transform>();
        fpsPistolObj = fpsPosition.transform.GetChild(0).GetComponent<Transform>();
        fpsRifleObj = fpsPosition.transform.GetChild(1).GetComponent<Transform>();  // �������� �̸� �ҷ��ͼ� ���α�
        fpsRifleObj.gameObject.SetActive(false);
        fpsWeapon = fpsPistolObj;

        SetWeapon(tpsPistol, fpsWeapon); // ���� ����
        weaponSlot = 1;       // ���� ���� ����
    }

    // Update is called once per frame
    void Update()
    {
        weaponPosition.position = targetObj.position;
        weaponPosition.rotation = targetObj.rotation;

        Shoot();
        Reload();
        WeaponSlots();
    }

    // ��� �Է�

    void Shoot()
    {
        RaycastHit hit;
        Vector3 hitPoint = cameraSet.followCam.transform.forward * 1f;
        if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range))
        {
            hitPoint = hit.point;
        }

        if (input.shoot && tpsPistol.ammo > 0)
        {
            if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range, layerMask))
            {
                GameObject hitObj = hit.transform.gameObject;
                Damage(hitObj);
                hitPoint = hit.point;
            }

            tpsPistol.ammo -= 1;
            PlayerUIManager.instance.SetAmmo(tpsPistol.ammo);   // ź �Ҹ�
            
            if (!cameraSet.tpsTest)
            { handAnimator.SetTrigger("isFire"); }
            animator.SetTrigger("isFire");

            input.shoot = false;
        }
        aimTarget.transform.position = hitPoint;    // �÷��̾� ���� ������
    }

    void Damage(GameObject _hitObj)
    {
        _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // ���񿡰� ������
    }
    // ����
    public void Reload()
    {
        if (input.reload)
        {
            
            if (0 < tpsPistol.totalAmmo && tpsPistol.ammo != tpsPistol.magazineSize && !isReloading)
            {
                Debug.Log("�������� �����Ѵ�.");
                isReloading = true;
                handAnimator.SetTrigger("isReload");
                animator.SetTrigger("isReload");
                handIKAmount = 0.5f;
                elbowIKAmount = 0.5f;
                StartCoroutine(ReloadStart());

            }
            input.reload = false;
        }
    }

    IEnumerator ReloadStart()
    {

        yield return new WaitForSeconds(reloadRate);

        float newAmmo = tpsPistol.ammo;
        tpsPistol.ammo = tpsPistol.ammo + tpsPistol.totalAmmo;
        if (tpsPistol.ammo > tpsPistol.magazineSize)
        {
            tpsPistol.ammo = tpsPistol.magazineSize;
        }

        tpsPistol.totalAmmo = tpsPistol.totalAmmo - (tpsPistol.magazineSize - newAmmo);
        if (0 > tpsPistol.totalAmmo)
        { tpsPistol.totalAmmo = 0; }

        PlayerUIManager.instance.SetAmmo(tpsPistol.ammo);
        PlayerUIManager.instance.SetTotalAmmo(tpsPistol.totalAmmo);
        isReloading = false;
        handIKAmount = 1f;
        elbowIKAmount = 1f;
        Debug.Log("�ٽ� ������ ����");

    }

    public void WeaponSlots()
    {
        WeaponSlot1();
        WeaponSlot2();
    }

    public void WeaponSlot1()
    {
        if (input.weaponSlot1 && weaponSlot == 2)
        {
            tpsRifle.gameObject.SetActive(false);
            tpsPistol.gameObject.SetActive(true);

            equipedWeapon = tpsPistol;
            rightHandObj = equipedWeapon.rightHandObj.transform;
            leftHandObj = equipedWeapon.leftHandObj.transform;
            rightElbowObj = equipedWeapon.rightElbowObj.transform;     // ������ �����Ȳ�ġ
            leftElbowObj = equipedWeapon.leftElbowObj.transform;     // ������ ���Ȳ�ġ
            weaponSlot = 1;

            if (!cameraSet.tpsTest)
            {
                fpsRifleObj.gameObject.SetActive(false);
                fpsPistolObj.gameObject.SetActive(true);

                fpsWeapon = fpsPistolObj;
                handAnimator = fpsWeapon.GetComponent<Animator>();
                playerMovement.fpsAnimator = handAnimator;
            }

            input.weaponSlot1 = false;
        }
    }

    public void WeaponSlot2()
    {
        if (input.weaponSlot2 && weaponSlot == 1)
        {
            tpsPistol.gameObject.SetActive(false);
            tpsRifle.gameObject.SetActive(true);

            equipedWeapon = tpsRifle;
            rightHandObj = equipedWeapon.rightHandObj.transform;
            leftHandObj = equipedWeapon.leftHandObj.transform;
            rightElbowObj = equipedWeapon.rightElbowObj.transform;     // ������ �����Ȳ�ġ
            leftElbowObj = equipedWeapon.leftElbowObj.transform;     // ������ ���Ȳ�ġ
            weaponSlot = 2;

            if (!cameraSet.tpsTest)
            {
                fpsPistolObj.gameObject.SetActive(false);
                fpsRifleObj.gameObject.SetActive(true);

                fpsWeapon = fpsRifleObj;
                handAnimator = fpsWeapon.GetComponent<Animator>();
                playerMovement.fpsAnimator = handAnimator;
            }
            input.weaponSlot2 = false;
        }
    }

    public void SetWeapon(Weapon _tpsweapon, Transform _fpsWeapon)
    {
        // ���� ���� �� TPS IK ����
        equipedWeapon = _tpsweapon;                         
        rightHandObj = equipedWeapon.rightHandObj.transform;     // ������ ������ �׷�
        leftHandObj = equipedWeapon.leftHandObj.transform;       // ������ �޼� �׷�
        rightElbowObj = equipedWeapon.rightElbowObj.transform;   // ������ �����Ȳ�ġ
        leftElbowObj = equipedWeapon.leftElbowObj.transform;     // ������ ���Ȳ�ġ

        // ���� ���� �� ���� ���� ����
        switch ((Type)equipedWeapon.weaponType)
        {
            case Type.Pistol:
                animator.SetBool("isWeaponPistol", true);
                animator.SetBool("isWeaponRifle", false);
                break;
            case Type.Rifle:
                animator.SetBool("isWeaponRifle", true);
                animator.SetBool("isWeaponPistol", false);
                break;
        }
        damage = equipedWeapon.damage;
        reloadRate = equipedWeapon.reloadRate;
        fireRate = equipedWeapon.fireRate;
        animator.SetFloat("ReloadSpeed", reloadRate);               // �ִϸ��̼� �߻�ӵ�
        PlayerUIManager.instance.SetAmmo(tpsPistol.ammo);           // ���� ź UI ����
        PlayerUIManager.instance.SetTotalAmmo(tpsPistol.totalAmmo); // ���� ���� ź UI ����

        handAnimator = _fpsWeapon.GetComponent<Animator>();
        playerMovement.fpsAnimator = handAnimator;
    }


    // ���� IK �ִϸ��̼� ó��
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // �÷��̾� lookat
                if (targetObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(targetObj.position);
                }
                // ������ �׷�
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                // �޼� �׷�
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
              
                // ���� �Ȳ�ġ
                if (leftElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, elbowIKAmount);
                }
                // ������ �Ȳ�ġ
                if (rightElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, elbowIKAmount);
                }
            }
            // �׷��� �ƹ��͵� ���ٸ� 0
            else
            {
                animator.SetLookAtWeight(0);

                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

                animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
            }
        }
    }
}