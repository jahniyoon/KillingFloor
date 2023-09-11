using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private CameraSetup cameraSet;
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11);


    public float damage = 10f;
    public float range = 100f;

    [Header("Animator IK")]
    protected Animator animator;
    public Animator handAnimator;

    public bool ikActive = false;
    public Transform weaponPosition = null;    // 무기 위치 기준점
    public Transform targetObj;             // 플레이어 시점

    [Header("TPS Weapon")]
    public Weapon tpsWeapon;
    public Transform rightHandObj = null;   // 오른손
    public Transform leftHandObj = null;    // 왼손
    private int weaponSlot;

    Weapon tpsPistol;    // 가져올 권총 무기 정보
    Weapon tpsRifle;     // 가져올 라이플 무기 정보

    [Header("FPS Weapon")]
    public Transform fpsPosition;
    public Transform fpsPistolObj;
    public Transform fpsRifleObj;
    public Transform fpsWeapon;


    // Start is called before the first frame update
    void Start()
    {
        cameraSet = GetComponent<CameraSetup>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        // TPS 무기 가져오기
        tpsPistol = weaponPosition.GetChild(0).GetComponent<Weapon>();
        tpsRifle = weaponPosition.GetChild(1).GetComponent<Weapon>();
        tpsPistol.gameObject.SetActive(false);    // 미리 꺼두기
        tpsRifle.gameObject.SetActive(false);    // 미리 꺼두기

        tpsWeapon = tpsPistol;                          // 기본총을 권총으로 장착
        rightHandObj = tpsWeapon.rightHandObj.transform;   // 권총의 오른손 그랩
        leftHandObj = tpsWeapon.leftHandObj.transform;     // 권총의 왼손 그랩
        weaponSlot = 1;                                 // 현재 슬롯 상태

        // FPS 무기 가져오기
        fpsPosition = transform.GetChild(0).GetChild(0).GetComponent<Transform>();
        fpsPistolObj = fpsPosition.transform.GetChild(0).GetComponent<Transform>();
        fpsRifleObj = fpsPosition.transform.GetChild(1).GetComponent<Transform>();  // 라이플은 미리 불러와서 꺼두기
        fpsRifleObj.gameObject.SetActive(false);

        fpsWeapon = fpsPistolObj;
        handAnimator = fpsWeapon.GetComponent<Animator>();
        playerMovement.fpsAnimator = handAnimator;

        
    }

    // Update is called once per frame
    void Update()
    {
        ActiveAnimation ();
    }

    // 사격 입력
    public void OnShoot()
    {
        handAnimator.SetTrigger("isFire");
        Shoot();
    }
    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range, layerMask))
        {
            Debug.DrawRay(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward * 100f, Color.green);
            GameObject hitObj = hit.transform.gameObject;
            Damage(hitObj);
        }
    }

    void Damage(GameObject _hitObj)
    {
        if (_hitObj.layer == 8) // 왼쪽
        {
            Debug.Log("왼쪽을 맞았다.");
            //Legacy:
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().healthBody -= 10f;
            _hitObj.transform.parent.parent.GetComponent<NormalZombieData>().health -= 10f;
        }
        else if (_hitObj.layer == 9) // 앞
        {
            //Legacy:
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().healthBody -= 10f;
            _hitObj.transform.parent.parent.GetComponent<NormalZombieData>().health -= 10f;

        }
        else if (_hitObj.layer == 10) // 오른쪽
        {
            Debug.Log("오른쪽을 맞았다.");
            //Legacy:
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().healthBody -= 10f;
            _hitObj.transform.parent.parent.GetComponent<NormalZombieData>().health -= 10f;

        }
        else if (_hitObj.layer == 11) // 헤드샷
        {
            Debug.Log("헤드샷");
            //Legacy:
            //_hitObj.transform.parent.parent.GetComponent<NormalZombieData>().healthHead -= 10f;
            _hitObj.transform.parent.parent.GetComponent<NormalZombieData>().health -= 10f * 1.5f;

        }
    }
    // 장전 입력
    public void OnReload()
    {
        handAnimator.SetTrigger("isReload");
    }

    public void OnWeaponSlot1()
    {
        if (weaponSlot == 2)
        {
            tpsRifle.gameObject.SetActive(false);
            tpsPistol.gameObject.SetActive(true);

            tpsWeapon = tpsPistol;
            rightHandObj = tpsWeapon.rightHandObj.transform;
            leftHandObj = tpsWeapon.leftHandObj.transform;
            weaponSlot = 1;

            fpsRifleObj.gameObject.SetActive(false);
            fpsPistolObj.gameObject.SetActive(true);

            fpsWeapon = fpsPistolObj;
            handAnimator = fpsWeapon.GetComponent<Animator>();
            playerMovement.fpsAnimator = handAnimator;

        }
    }

    public void OnWeaponSlot2()
    {
        if (weaponSlot == 1)
        {
            tpsPistol.gameObject.SetActive(false);
            tpsRifle.gameObject.SetActive(true);

            tpsWeapon = tpsRifle;
            rightHandObj = tpsWeapon.rightHandObj.transform;
            leftHandObj = tpsWeapon.leftHandObj.transform;
            weaponSlot = 2;

            fpsPistolObj.gameObject.SetActive(false);
            fpsRifleObj.gameObject.SetActive(true);

            fpsWeapon = fpsRifleObj;
            handAnimator = fpsWeapon.GetComponent<Animator>();
            playerMovement.fpsAnimator = handAnimator;


        }
    }

    public void ActiveAnimation()
    {
      
    }


    // 무기 애니메이션 처리
    void OnAnimatorIK()
    {
        weaponPosition.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow);
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
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                // 왼손 그랩
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
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
            }
        }
    }
}
