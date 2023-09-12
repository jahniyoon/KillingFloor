using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using UnityEngine.WSA;
using static UnityEditor.Progress;
using static UnityEngine.ParticleSystem;
using static UnityEngine.UI.Image;

public class PlayerShooter : MonoBehaviour
{
    public enum Type { Pistol, Rifle, Melee, Heal };
    private PlayerInputs input;
    private PlayerMovement playerMovement;
    private CameraSetup cameraSet;
    protected Animator animator;
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11);    // 데미지 받을 좀비의 레이어 마스크


    public Transform aimTarget; // 플레이어가 보는 방향
    public Transform targetObj;                // 플레이어 시점
    public Transform weaponPosition = null;    // 무기 위치 기준점
    public Transform rightHandPosition; // 오른손 위치
    public bool isAnimation;

    [Header("Weapon Info")]
    public Type weaponType;
    public Weapon equipedWeapon;
    [Range(0, 5)]
    public int weaponSlot;
    public float damage;        // 총기 데미지
    public float range = 100f;  // 사거리
    public float reloadRate;    // 재장전 속도
    public float fireRate;      // 사격 속도
    public bool isReloading;    // 가능 여부
    public bool isFireReady;

    
    [Header("TPS Weapon")]
    Weapon tpsPistol;    // 가져올 권총 무기 정보
    Weapon tpsRifle;     // 가져올 라이플 무기 정보

    [Header("FPS Weapon")]
    public Transform fpsPosition;
    public Transform fpsPistol;
    public Transform fpsRifle;
    public Transform fpsWeapon;

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
        cameraSet = GetComponent<CameraSetup>();
        animator = GetComponent<Animator>();

        // TPS 무기 가져오기
        tpsPistol = weaponPosition.GetChild(0).GetComponent<Weapon>();
        tpsRifle = weaponPosition.GetChild(1).GetComponent<Weapon>();
        tpsRifle.gameObject.SetActive(false);    // 미리 꺼두기

        // FPS 무기 가져오기
        fpsPosition = transform.GetChild(0).GetChild(0).GetComponent<Transform>();
        fpsPistol = fpsPosition.transform.GetChild(0).GetComponent<Transform>();
        fpsRifle = fpsPosition.transform.GetChild(1).GetComponent<Transform>();  // 라이플은 미리 불러와서 꺼두기
        fpsRifle.gameObject.SetActive(false);

        SetWeapon(tpsPistol, fpsPistol); // 무기 장착
        animator.SetBool("isWeaponPistol", true);
        animator.SetBool("isWeaponRifle", false);
        weaponSlot = 1;       // 현재 슬롯 상태
    }

    // Update is called once per frame
    void Update()
    {
        HandSet();
        Shoot();
        Reload();
        WeaponSlots();
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
        if (input.shoot && equipedWeapon.ammo > 0 && !isReloading && !isFireReady)
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
            if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range, layerMask))
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
            input.shoot = false;
        }
        // 남은 총알이 있을 때
        else if (input.shoot && equipedWeapon.ammo == 0 && equipedWeapon.remainingAmmo >= 0)
        {
            input.reload = true; // 재장전 버튼 눌러주기
            input.shoot = false;
        }

        // 남은 총알도 없을 때
        else if(input.shoot && equipedWeapon.ammo == 0 && equipedWeapon.remainingAmmo == 0)
        {
            // ToDo : 틱 사운드 플레이되도록 하기 (총알 없음)

            input.shoot = false;
        }
        aimTarget.transform.position = hitPoint;    // 플레이어 조준 포지션
    }
    IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(1 / (fireRate/60)); // fireRate 는 RPM
        Debug.Log("총 쏠수 있다");
        handIKAmount = 1f;
        elbowIKAmount = 1f;
        equipedWeapon.ammo -= 1;
        PlayerUIManager.instance.SetAmmo(equipedWeapon.ammo);   // 탄 소모
        isFireReady = false;
        isAnimation = false;

    }
    void Damage(GameObject _hitObj)
    {
        //_hitObj.transform.GetComponent<HitPoint>().Hit(damage); // 좀비에게 데미지
    }

    // 장전
    public void Reload()
    {
        if (input.reload)
        {
            // 잔여 탄이 0보다 많고, 탄이 꽉차있지 않고, 장전 가능할 때 장전
            if (0 < equipedWeapon.remainingAmmo && equipedWeapon.ammo != equipedWeapon.magazineSize && !isReloading)
            {
                isReloading = true;
                isAnimation = true;
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
        PlayerUIManager.instance.SetTotalAmmo(equipedWeapon.remainingAmmo);

        handIKAmount = 1f;
        elbowIKAmount = 1f;
        isReloading = false;
        isAnimation = false;
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
            fpsRifle.gameObject.SetActive(false);
            fpsPistol.gameObject.SetActive(true);

            SetWeapon(tpsPistol, fpsPistol); // 무기 장착
            weaponSlot = 1;
            animator.SetBool("isWeaponPistol", true);
            animator.SetBool("isWeaponRifle", false);

            input.weaponSlot1 = false;
        }
    }

    public void WeaponSlot2()
    {
        if (input.weaponSlot2 && weaponSlot == 1)
        {
            tpsPistol.gameObject.SetActive(false);
            tpsRifle.gameObject.SetActive(true);
            fpsPistol.gameObject.SetActive(false);
            fpsRifle.gameObject.SetActive(true);

            SetWeapon(tpsRifle, fpsRifle); // 무기 장착
            weaponSlot = 2;
            animator.SetBool("isWeaponPistol",false);
            animator.SetBool("isWeaponRifle",true);

            input.weaponSlot2 = false;
        }
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

        // 애니메이션 발사속도
        PlayerUIManager.instance.SetAmmo(_tpsweapon.ammo);           // 현재 탄 UI 세팅
        PlayerUIManager.instance.SetTotalAmmo(_tpsweapon.remainingAmmo); // 현재 남은 탄 UI 세팅

        // FPS상태도 세팅
        fpsWeapon = _fpsWeapon;
        handAnimator = _fpsWeapon.GetComponent<Animator>();
        handAnimator.SetFloat("ReloadSpeed", reloadRate);
        playerMovement.fpsAnimator = handAnimator;

    }
    void HandSet()
    {
        //if (!isAnimation)
        //{
        //    weaponPosition.position = targetObj.position;
        //    weaponPosition.rotation = targetObj.rotation;
        //}
        //else
        //{
        //    weaponPosition.position = rightHandPosition.position;
        //    weaponPosition.rotation = rightHandPosition.rotation;
        //}
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