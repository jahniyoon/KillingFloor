using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{

    [Header("Animator IK")]
    protected Animator animator;
    public Animator handAnimator;

    public bool ikActive = false;
    public Transform weaponPosition = null;    // 무기 위치 기준점
    public Transform targetObj;             // 플레이어 시점

    [Header("Weapon")]
    public Weapon weapon;
    public Transform rightHandObj = null;   // 오른손
    public Transform leftHandObj = null;    // 왼손
    private int weaponSlot;

    Weapon pistolWeapon;
    Weapon rifleWeapon;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        // 무기 가져오기
        pistolWeapon = weaponPosition.GetChild(0).GetComponent<Weapon>();
        rifleWeapon = weaponPosition.GetChild(1).GetComponent<Weapon>();
        rifleWeapon.gameObject.SetActive(false);    // 미리 꺼두기

        weapon = pistolWeapon;
        rightHandObj = weapon.rightHandObj.transform;
        leftHandObj = weapon.leftHandObj.transform;
        weaponSlot = 1;
    }

    // Update is called once per frame
    void Update()
    {
        ActiveAnimation ();
    }

    // 사격 입력
    public void OnShoot()
    {
        if (weapon != null)
        handAnimator.SetTrigger("isFire");
    }
    // 장전 입력
    public void OnReload()
    {
        if (weapon != null)
            handAnimator.SetTrigger("isReload");
    }

    public void OnWeaponSlot1()
    {
        Debug.Log("1번 무기로 변경");
        if (weaponSlot == 2)
        {
            rifleWeapon.gameObject.SetActive(false);
            pistolWeapon.gameObject.SetActive(true);

            weapon = pistolWeapon;
            rightHandObj = weapon.rightHandObj.transform;
            leftHandObj = weapon.leftHandObj.transform;
            weaponSlot = 1;
        }
    }

    public void OnWeaponSlot2()
    {
        Debug.Log("2번 무기로 변경");
        if (weaponSlot == 1)
        {
            pistolWeapon.gameObject.SetActive(false);
            rifleWeapon.gameObject.SetActive(true);

            weapon = rifleWeapon;
            rightHandObj = weapon.rightHandObj.transform;
            leftHandObj = weapon.leftHandObj.transform;
            weaponSlot = 2;
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
