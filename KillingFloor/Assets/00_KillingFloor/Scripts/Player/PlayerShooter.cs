using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{

    [Header("Animator IK")]
    protected Animator animator;
    public Animator handAnimator;

    public bool ikActive = false;
    public Transform weaponPosition = null;    // ���� ��ġ ������
    public Transform targetObj;             // �÷��̾� ����

    [Header("Weapon")]
    public Weapon weapon;
    public Transform rightHandObj = null;   // ������
    public Transform leftHandObj = null;    // �޼�

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        weapon = weaponPosition.GetChild(0).GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        ActiveAnimation ();
    }

    public void OnShoot()
    {
        if (weapon != null)
        handAnimator.SetTrigger("isFire");
    }

    public void OnReload()
    {
        if (weapon != null)
            handAnimator.SetTrigger("isReload");
    }
    public void ActiveAnimation()
    {
    }


    void OnAnimatorIK()
    {
        weaponPosition.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow);
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
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                // �޼� �׷�
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
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
            }
        }
    }
}
