using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;


public class PlayerShooter : MonoBehaviour
{
    public enum Type { Pistol, Rifle, Melee, Heal };
    private PlayerInputs input;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private CameraSetup cameraSet;
    protected Animator animator;
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11);    // ������ ���� ������ ���̾� ����ũ


    public Transform aimTarget; // �÷��̾ ���� ����
    public Transform targetObj;                // �÷��̾� ����
    public Transform weaponPosition = null;    // ���� ��ġ ������
    public Transform rightHandPosition; // ������ ��ġ
    public bool isAnimation;    // ToDo ���� ����ȭ �ʿ�

    [Header("Weapon Info")]
    public Weapon equipedWeapon;
    [Range(1, 5)]
    public int weaponSlot = 1;
    public float damage;        // �ѱ� ������
    public float range = 100f;  // ��Ÿ�
    public float reloadRate;    // ������ �ӵ�
    public float fireRate;      // ��� �ӵ�
    public bool isReloading;    // ���� ����
    public bool isFireReady;
    public int grenade;
    public bool isGrenade;
    public float healCoolDown = 15f;  // �� ��ٿ�

    
    [Header("TPS Weapon")]
    Weapon tpsPistol;    // ������ ���� ���� ����
    Weapon tpsRifle;     // ������ ������ ���� ����
    Weapon tpsMelee;     // ������ ���� ���� ����
    Weapon tpsHeal;     // ������ ���� ���� ����

    [Header("FPS Weapon")]
    public Transform fpsPosition;
    public Transform fpsPistol;
    public Transform fpsRifle;
    public Transform fpsMelee;
    public Transform fpsHeal;
    public Transform fpsGrenade;

    [Header("Animator IK")]
    public Animator handAnimator;
    public Transform rightHandObj = null;   // ������
    public Transform leftHandObj = null;    // �޼�
    public Transform rightElbowObj = null;   // ������ �׷�
    public Transform leftElbowObj = null;    // �޼� �׷�
    [Range(0, 1)]
    public float handIKAmount = 1;
    [Range(0, 1)]
    public float elbowIKAmount = 1;
    [Range(0, 1)]
    public float animationIKAmount = 0.5f; // �ִϸ��̼� �� IK �⺻��
    public bool ikActive = false;

    public GameObject bulletHole;


    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInputs>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        cameraSet = GetComponent<CameraSetup>();
        animator = GetComponent<Animator>();

        // TPS ���� ��������
        tpsPistol = weaponPosition.GetChild(0).GetComponent<Weapon>();
        tpsRifle = weaponPosition.GetChild(1).GetComponent<Weapon>();
        tpsMelee = weaponPosition.GetChild(2).GetComponent<Weapon>();
        tpsHeal = weaponPosition.GetChild(3).GetComponent<Weapon>();
        tpsRifle.gameObject.SetActive(false);    // �̸� ���α�
        tpsMelee.gameObject.SetActive(false);    // �̸� ���α�
        tpsHeal.gameObject.SetActive(false);    // �̸� ���α�

        // FPS ���� ��������
        fpsPosition = transform.GetChild(0).GetChild(0).GetComponent<Transform>();
        fpsPistol = fpsPosition.transform.GetChild(0).GetComponent<Transform>();
        fpsRifle = fpsPosition.transform.GetChild(1).GetComponent<Transform>();  // �������� �̸� �ҷ��ͼ� ���α�
        fpsMelee = fpsPosition.transform.GetChild(2).GetComponent<Transform>();  
        fpsHeal = fpsPosition.transform.GetChild(3).GetComponent<Transform>();  
        fpsGrenade = fpsPosition.transform.GetChild(4).GetComponent<Transform>();  

        fpsRifle.gameObject.SetActive(false);
        fpsMelee.gameObject.SetActive(false);
        fpsHeal.gameObject.SetActive(false);
        fpsGrenade.gameObject.SetActive(false);


        SetWeapon(tpsPistol, fpsPistol); // ���� ����
        animator.SetBool("isWeaponPistol", true);
        animator.SetBool("isWeaponRifle", false);
    }

    // Update is called once per frame
    void Update()
    {
        // �Է� ���ɿ��� Ȯ��
        if (GameManager.instance != null && GameManager.instance.inputEnable)
        {
            HandSet();
            Aim();
            Shoot();
            Reload();
            Weapons();
            Heal();
        }
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
        // ���
        if (input.shoot && 0 < equipedWeapon.ammo&& !isReloading && !isFireReady && weaponSlot < 3 && !input.dash)
        {
            isFireReady = true;
            isAnimation = true;
            // �ִϸ��̼� �۵� �� ��� IK Ǯ���ֱ�
            handAnimator.SetTrigger("isFire");
            animator.SetTrigger("isFire");
            handIKAmount = animationIKAmount;
            elbowIKAmount = animationIKAmount;
            StartCoroutine(ShootCoroutine());
    
            // ���� ������ ������ ������
            if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range, layerMask) )
            {
                GameObject hitObj = hit.transform.gameObject;
                Damage(hitObj);
                hitPoint = hit.point;

            }
            else if(Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range))
            {
                hitPoint = hit.point;

                GameObject particles = (GameObject)Instantiate(bulletHole);
                particles.transform.position = hit.point;
                Destroy(particles, 8f);
            }
            if (weaponSlot == 1)
            {
                input.shoot = false;
            }

        }
        // ���� �Ѿ��� ���� ��
        else if (input.shoot && equipedWeapon.ammo == 0 && equipedWeapon.remainingAmmo >= 0 && !input.dash)
        {
            input.reload = true; // ������ ��ư �����ֱ�
            input.shoot = false;
        }

        // ���� �Ѿ˵� ���� ��
        else if(input.shoot && equipedWeapon.ammo == 0 && equipedWeapon.remainingAmmo == 0 && !input.dash)
        {
            // ToDo : ƽ ���� �÷��̵ǵ��� �ϱ� (�Ѿ� ����)

            input.shoot = false;
        }

        // ��������
        if(input.shoot && weaponSlot == 3 && !isFireReady && !input.dash)
        {
            isFireReady = true;
            handAnimator.SetTrigger("isFire");
            StartCoroutine(WeaponDelay(reloadRate));
            input.shoot = false;
        }

        // ��
        if (input.shoot && weaponSlot == 4 && !isFireReady && 15 <= healCoolDown && playerHealth.health != 100 && !input.dash)
        {
            isFireReady = true;
            handAnimator.SetTrigger("isFire");
            healCoolDown = -0.1f;
            StartCoroutine(WeaponDelay(reloadRate));
            playerHealth.RestoreHealth(damage);
            input.shoot = false;
        }
        aimTarget.transform.position = hitPoint;    // �÷��̾� ���� ������
    }

    void Aim()
    {
        if (input.dash)
        {
            if (weaponSlot <= 2)
            {
                handAnimator.SetBool("isAim", false);
            }
            return; 
        }

        if (weaponSlot <= 2 && !isReloading && !input.dash)
        {
            handAnimator.SetBool("isAim", input.aim);
        }
        if (weaponSlot == 3 && !isFireReady && input.aim)
        {
            input.aim = false;
            isFireReady = true;
            handAnimator.SetTrigger("isAim");
            StartCoroutine(WeaponDelay(reloadRate * 2));
        }


    }

    IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(1 / (fireRate/90)); // fireRate �� RPM
        handIKAmount = 1f;
        elbowIKAmount = 1f;
        equipedWeapon.ammo -= 1;
        PlayerUIManager.instance.SetAmmo(equipedWeapon.ammo);   // ź �Ҹ�
        isFireReady = false;
        isAnimation = false;

    }

    IEnumerator WeaponDelay(float _reloadRate)
    {
        yield return new WaitForSeconds(_reloadRate);
        isFireReady = false;
        isAnimation = false;

    }
    void Heal()
    {
        if(healCoolDown <= 15)
        {
            healCoolDown += Time.deltaTime;
            PlayerUIManager.instance.SetHeal(healCoolDown);
        }

    }
    void Damage(GameObject _hitObj)
    {
        if (_hitObj.transform.GetComponent<HitPoint>() == null)
        {
            playerHealth.GetCoin(100);  // Debug ����׿� ��ȭ ȹ��
            return;
        }
        if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
        {
            _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // ���񿡰� ������

            // ���� ���� �״´ٸ�
            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
            {
                // ���� ���̰�
                playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                // ���ΰ� �ʱ�ȭ
                _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
            }
        }
    }

    // ����
    public void Reload()
    {
        if (input.reload && weaponSlot < 3)
        {
            // �ܿ� ź�� 0���� ����, ź�� �������� �ʰ�, ���� ������ �� ����
            if (0 < equipedWeapon.remainingAmmo && equipedWeapon.ammo != equipedWeapon.magazineSize && !isReloading)
            {
                isReloading = true;
                isAnimation = true;
                input.dash = false;

                // �ִϸ��̼� �۵� �� ��� IK Ǯ���ֱ�
                handAnimator.SetTrigger("isReload");
                animator.SetTrigger("isReload");
                handIKAmount = animationIKAmount;
                elbowIKAmount = animationIKAmount;
                StartCoroutine(ReloadCoroutine());
            }
            input.reload = false;
        }
    }
    // ���� �ڷ�ƾ
    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadRate);

        float currentAmmo = equipedWeapon.ammo;
        float remainingAmmo = equipedWeapon.remainingAmmo - (equipedWeapon.magazineSize - currentAmmo);

        equipedWeapon.ammo = Mathf.Min(equipedWeapon.magazineSize, equipedWeapon.ammo + equipedWeapon.remainingAmmo);   // ���� ź ����
        equipedWeapon.remainingAmmo = Mathf.Max(0, remainingAmmo);                                                      // ���� ź ����

        PlayerUIManager.instance.SetAmmo(equipedWeapon.ammo);
        PlayerUIManager.instance.SetRemainingAmmo(equipedWeapon.remainingAmmo);

        handIKAmount = 1f;
        elbowIKAmount = 1f;
        isReloading = false;
        isAnimation = false;
    }


    public void Weapons()
    {
        WeaponInput();

        if(weaponSlot == 1 && !isGrenade)
        {
            tpsRifle.gameObject.SetActive(false);
            tpsPistol.gameObject.SetActive(true);
            fpsRifle.gameObject.SetActive(false);
            fpsPistol.gameObject.SetActive(true);
            fpsMelee.gameObject.SetActive(false);
            fpsHeal.gameObject.SetActive(false);
            fpsGrenade.gameObject.SetActive(false);
            SetWeapon(tpsPistol, fpsPistol); // ���� ����
            animator.SetBool("isWeaponPistol", true);
            animator.SetBool("isWeaponRifle", false);
        }
        if(weaponSlot == 2 && !isGrenade)
        {
            tpsPistol.gameObject.SetActive(false);
            tpsRifle.gameObject.SetActive(true);
            fpsPistol.gameObject.SetActive(false);
            fpsRifle.gameObject.SetActive(true);
            fpsMelee.gameObject.SetActive(false);
            fpsHeal.gameObject.SetActive(false);
            fpsGrenade.gameObject.SetActive(false);
            SetWeapon(tpsRifle, fpsRifle); // ���� ����
            animator.SetBool("isWeaponPistol", false);
            animator.SetBool("isWeaponRifle", true);
        }
        if (weaponSlot == 3 && !isGrenade)
        {
            tpsPistol.gameObject.SetActive(false);
            tpsRifle.gameObject.SetActive(false);
            fpsPistol.gameObject.SetActive(false);
            fpsRifle.gameObject.SetActive(false);
            fpsMelee.gameObject.SetActive(true);
            fpsHeal.gameObject.SetActive(false);
            fpsGrenade.gameObject.SetActive(false);
            SetWeapon(tpsMelee, fpsMelee); // ���� ����
        }
        if (weaponSlot == 4 && !isGrenade)
        {
            tpsPistol.gameObject.SetActive(false);
            tpsRifle.gameObject.SetActive(false);
            fpsPistol.gameObject.SetActive(false);
            fpsRifle.gameObject.SetActive(false);
            fpsMelee.gameObject.SetActive(false);
            fpsHeal.gameObject.SetActive(true);
            fpsGrenade.gameObject.SetActive(false);
            SetWeapon(tpsHeal, fpsHeal); // ���� ����
        }
        if(isGrenade)
        {
            tpsPistol.gameObject.SetActive(false);
            tpsRifle.gameObject.SetActive(false);
            fpsPistol.gameObject.SetActive(false);
            fpsRifle.gameObject.SetActive(false);
            fpsMelee.gameObject.SetActive(false);
            fpsHeal.gameObject.SetActive(false);
            fpsGrenade.gameObject.SetActive(true);

        }

    }

    public void WeaponInput()
    {
        if (input.weaponSlot1)
        {
            if (weaponSlot != 1)
            {
                weaponSlot = 1;
            }
            input.weaponSlot1 = false;
        }
        if (input.weaponSlot2)
        {
            if (weaponSlot != 2)
            {
                weaponSlot = 2;
            }
            input.weaponSlot2 = false;
        }
        if (input.weaponSlot3)
        {
            if (weaponSlot != 3)
            {
                weaponSlot = 3;
            }
            input.weaponSlot3 = false;
        }
        if (input.weaponSlot4)
        {
            if (weaponSlot != 4)
            {
                weaponSlot = 4;
            }
            input.weaponSlot4 = false;
        }
        if(input.grenade)
        {
            if (!isGrenade && 0 < grenade)
            { 
                isGrenade = true;
                isFireReady = true;
                StartCoroutine(Grenade());
            }
            input.grenade = false;
        }

        if (input.scroll > 0)
        {
            weaponSlot += 1;
            if (4 < weaponSlot) weaponSlot = 1;
        }
        if (input.scroll < 0)
        {
            weaponSlot -= 1;
            if (0 >= weaponSlot) weaponSlot = 4;
        }
    }
    IEnumerator Grenade()
    {
        yield return new WaitForSeconds(2.1f);
        grenade -= 1;
        PlayerUIManager.instance.SetGrenade(grenade);
        isGrenade = false;
        isFireReady = false;

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
                equipedWeapon.weaponType = Weapon.Type.Pistol;
                break;
            case Type.Rifle:
                animator.SetBool("isWeaponRifle", true);
                animator.SetBool("isWeaponPistol", false);
                equipedWeapon.weaponType = Weapon.Type.Rifle;
                break;
        }
        damage = equipedWeapon.damage;
        reloadRate = equipedWeapon.reloadRate;
        fireRate = equipedWeapon.fireRate;
        animator.SetFloat("ReloadSpeed", reloadRate);

        PlayerUIManager.instance.SetAmmo(_tpsweapon.ammo);           // ���� ź UI ����
        PlayerUIManager.instance.SetRemainingAmmo(_tpsweapon.remainingAmmo); // ���� ���� ź UI ����
        PlayerUIManager.instance.SetGrenade(grenade);

        // FPS���µ� ����
        handAnimator = _fpsWeapon.GetComponent<Animator>();
        handAnimator.SetFloat("ReloadSpeed", reloadRate);
        playerMovement.fpsAnimator = handAnimator;

    }
    public void GetAmmo(int value)
    {
        equipedWeapon.remainingAmmo += value;
        PlayerUIManager.instance.SetRemainingAmmo(equipedWeapon.remainingAmmo);
    }
    void HandSet()
    {
        weaponPosition.position = targetObj.position;
        weaponPosition.rotation = targetObj.rotation;
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