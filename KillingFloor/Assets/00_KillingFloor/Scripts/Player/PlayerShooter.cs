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
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11);    // 데미지 받을 좀비의 레이어 마스크


    public Transform aimTarget; // 플레이어가 보는 방향
    public Transform targetObj;                // 플레이어 시점
    public Transform weaponPosition = null;    // 무기 위치 기준점
    public Transform rightHandPosition; // 오른손 위치
    public bool isAnimation;    // ToDo 구조 최적화 필요

    [Header("Weapon Info")]
    public Weapon equipedWeapon;
    [Range(1, 5)]
    public int weaponSlot = 1;
    public float damage;        // 총기 데미지
    public float range = 100f;  // 사거리
    public float reloadRate;    // 재장전 속도
    public float fireRate;      // 사격 속도
    public bool isReloading;    // 가능 여부
    public bool isFireReady;
    public int grenade;
    public bool isGrenade;
    public float healCoolDown = 15f;  // 힐 쿨다운

    
    [Header("TPS Weapon")]
    Weapon tpsPistol;    // 가져올 권총 무기 정보
    Weapon tpsRifle;     // 가져올 라이플 무기 정보
    Weapon tpsMelee;     // 가져올 근접 무기 정보
    Weapon tpsHeal;     // 가져올 근접 무기 정보

    [Header("FPS Weapon")]
    public Transform fpsPosition;
    public Transform fpsPistol;
    public Transform fpsRifle;
    public Transform fpsMelee;
    public Transform fpsHeal;
    public Transform fpsGrenade;

    [Header("Animator IK")]
    public Animator handAnimator;
    public Transform rightHandObj = null;   // 오른손
    public Transform leftHandObj = null;    // 왼손
    public Transform rightElbowObj = null;   // 오른손 그랩
    public Transform leftElbowObj = null;    // 왼손 그랩
    [Range(0, 1)]
    public float handIKAmount = 1;
    [Range(0, 1)]
    public float elbowIKAmount = 1;
    [Range(0, 1)]
    public float animationIKAmount = 0.5f; // 애니메이션 중 IK 기본값
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

        // TPS 무기 가져오기
        tpsPistol = weaponPosition.GetChild(0).GetComponent<Weapon>();
        tpsRifle = weaponPosition.GetChild(1).GetComponent<Weapon>();
        tpsMelee = weaponPosition.GetChild(2).GetComponent<Weapon>();
        tpsHeal = weaponPosition.GetChild(3).GetComponent<Weapon>();
        tpsRifle.gameObject.SetActive(false);    // 미리 꺼두기
        tpsMelee.gameObject.SetActive(false);    // 미리 꺼두기
        tpsHeal.gameObject.SetActive(false);    // 미리 꺼두기

        // FPS 무기 가져오기
        fpsPosition = transform.GetChild(0).GetChild(0).GetComponent<Transform>();
        fpsPistol = fpsPosition.transform.GetChild(0).GetComponent<Transform>();
        fpsRifle = fpsPosition.transform.GetChild(1).GetComponent<Transform>();  // 라이플은 미리 불러와서 꺼두기
        fpsMelee = fpsPosition.transform.GetChild(2).GetComponent<Transform>();  
        fpsHeal = fpsPosition.transform.GetChild(3).GetComponent<Transform>();  
        fpsGrenade = fpsPosition.transform.GetChild(4).GetComponent<Transform>();  

        fpsRifle.gameObject.SetActive(false);
        fpsMelee.gameObject.SetActive(false);
        fpsHeal.gameObject.SetActive(false);
        fpsGrenade.gameObject.SetActive(false);


        SetWeapon(tpsPistol, fpsPistol); // 무기 장착
        animator.SetBool("isWeaponPistol", true);
        animator.SetBool("isWeaponRifle", false);
    }

    // Update is called once per frame
    void Update()
    {
        // 입력 가능여부 확인
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

    // 사격 입력

    void Shoot()
    {
        RaycastHit hit;
        Vector3 hitPoint = cameraSet.followCam.transform.forward * 1f;
        if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range))
        {
            hitPoint = hit.point;
        }
        // 사격
        if (input.shoot && 0 < equipedWeapon.ammo&& !isReloading && !isFireReady && weaponSlot < 3 && !input.dash)
        {
            isFireReady = true;
            isAnimation = true;
            // 애니메이션 작동 후 잠깐 IK 풀어주기
            handAnimator.SetTrigger("isFire");
            animator.SetTrigger("isFire");
            handIKAmount = animationIKAmount;
            elbowIKAmount = animationIKAmount;
            StartCoroutine(ShootCoroutine());
    
            // 만약 닿은게 있으면 데미지
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
        // 남은 총알이 있을 때
        else if (input.shoot && equipedWeapon.ammo == 0 && equipedWeapon.remainingAmmo >= 0 && !input.dash)
        {
            input.reload = true; // 재장전 버튼 눌러주기
            input.shoot = false;
        }

        // 남은 총알도 없을 때
        else if(input.shoot && equipedWeapon.ammo == 0 && equipedWeapon.remainingAmmo == 0 && !input.dash)
        {
            // ToDo : 틱 사운드 플레이되도록 하기 (총알 없음)

            input.shoot = false;
        }

        // 근접공격
        if(input.shoot && weaponSlot == 3 && !isFireReady && !input.dash)
        {
            isFireReady = true;
            handAnimator.SetTrigger("isFire");
            StartCoroutine(WeaponDelay(reloadRate));
            input.shoot = false;
        }

        // 힐
        if (input.shoot && weaponSlot == 4 && !isFireReady && 15 <= healCoolDown && playerHealth.health != 100 && !input.dash)
        {
            isFireReady = true;
            handAnimator.SetTrigger("isFire");
            healCoolDown = -0.1f;
            StartCoroutine(WeaponDelay(reloadRate));
            playerHealth.RestoreHealth(damage);
            input.shoot = false;
        }
        aimTarget.transform.position = hitPoint;    // 플레이어 조준 포지션
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
        yield return new WaitForSeconds(1 / (fireRate/90)); // fireRate 는 RPM
        handIKAmount = 1f;
        elbowIKAmount = 1f;
        equipedWeapon.ammo -= 1;
        PlayerUIManager.instance.SetAmmo(equipedWeapon.ammo);   // 탄 소모
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
            playerHealth.GetCoin(100);  // Debug 디버그용 재화 획득
            return;
        }
        if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
        {
            _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // 좀비에게 데미지

            // 만약 좀비가 죽는다면
            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
            {
                // 코인 먹이고
                playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                // 코인값 초기화
                _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
            }
        }
    }

    // 장전
    public void Reload()
    {
        if (input.reload && weaponSlot < 3)
        {
            // 잔여 탄이 0보다 많고, 탄이 꽉차있지 않고, 장전 가능할 때 장전
            if (0 < equipedWeapon.remainingAmmo && equipedWeapon.ammo != equipedWeapon.magazineSize && !isReloading)
            {
                isReloading = true;
                isAnimation = true;
                input.dash = false;

                // 애니메이션 작동 후 잠깐 IK 풀어주기
                handAnimator.SetTrigger("isReload");
                animator.SetTrigger("isReload");
                handIKAmount = animationIKAmount;
                elbowIKAmount = animationIKAmount;
                StartCoroutine(ReloadCoroutine());
            }
            input.reload = false;
        }
    }
    // 장전 코루틴
    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadRate);

        float currentAmmo = equipedWeapon.ammo;
        float remainingAmmo = equipedWeapon.remainingAmmo - (equipedWeapon.magazineSize - currentAmmo);

        equipedWeapon.ammo = Mathf.Min(equipedWeapon.magazineSize, equipedWeapon.ammo + equipedWeapon.remainingAmmo);   // 현재 탄 세팅
        equipedWeapon.remainingAmmo = Mathf.Max(0, remainingAmmo);                                                      // 남은 탄 세팅

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
            SetWeapon(tpsPistol, fpsPistol); // 무기 장착
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
            SetWeapon(tpsRifle, fpsRifle); // 무기 장착
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
            SetWeapon(tpsMelee, fpsMelee); // 무기 장착
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
            SetWeapon(tpsHeal, fpsHeal); // 무기 장착
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
        // 무기 장착 및 TPS IK 세팅
        equipedWeapon = _tpsweapon;                         
        rightHandObj = equipedWeapon.rightHandObj.transform;     // 권총의 오른손 그랩
        leftHandObj = equipedWeapon.leftHandObj.transform;       // 권총의 왼손 그랩
        rightElbowObj = equipedWeapon.rightElbowObj.transform;   // 권총의 오른팔꿈치
        leftElbowObj = equipedWeapon.leftElbowObj.transform;     // 권총의 왼팔꿈치

        // 무기 장착 및 무기 정보 세팅
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

        PlayerUIManager.instance.SetAmmo(_tpsweapon.ammo);           // 현재 탄 UI 세팅
        PlayerUIManager.instance.SetRemainingAmmo(_tpsweapon.remainingAmmo); // 현재 남은 탄 UI 세팅
        PlayerUIManager.instance.SetGrenade(grenade);

        // FPS상태도 세팅
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

    // 무기 IK 애니메이션 처리
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // 플레이어 lookat
                if (targetObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(targetObj.position);
                }
                // 오른손 그랩
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                // 왼손 그랩
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
              
                // 왼쪽 팔꿈치
                if (leftElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, elbowIKAmount);
                }
                // 오른쪽 팔꿈치
                if (rightElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, elbowIKAmount);
                }
            }
            // 그랩에 아무것도 없다면 0
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