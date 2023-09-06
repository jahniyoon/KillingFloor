using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerSetting; // �÷��̾� �� ���� Ŭ����


public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    public Animator tpsAnimator;
    public Animator fpsAnimator;
    private PlayerAction playerInput;                    // ��ǲ �ý��� Ȱ��� �̸� �����ؾ���

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;
    public Transform cameraHolder; // ī�޶� ���� �� (�÷��̾ ���� ���⿡ �°� ī�޶� ���� ����)
    public Transform lookTarget; // ī�޶� ���� �� (�÷��̾ ���� ���⿡ �°� ī�޶� ���� ����)
    public Transform gunPosition;

    [Header("Player")]
    public Vector2 inputMoveVec;
    public Vector2 inputViewVec;

    private float playerJumpVec;      // ���� ���� ��
    private float playerSpeed;  // �÷��̾��� ���� �ӵ�

    public float walkSpeed;     // �÷��̾� �ȴ� �ӵ�
    public float dashSpeed;     // �÷��̾� ��� �ӵ�
    public float rotaionSpeed;  // ȸ�� �ӵ�
    public float jumpForce;     // ���� ��
    public float gravity;       // �߷°�

    private float dash;         // ��ù�ư �Է� Ȯ��
    private bool isDash;        // ��� �����ΰ�
    private bool isGrounded = true;    // �ٴ��ΰ�
    public bool isDie;          // �׾��� ToDo : ���� ��ƼƼ�� �̵�


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Animator tpsAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;   // ���콺 ����

        if (!isDie)
        {
            CalculateMovement();    // �÷��̾� �̵� ���
            CalculateView();        // �÷��̾� ���� ���
            ActiveAnimation();      // �ִϸ��̼� ����
        }
    }
    // �÷��̾��� �̵� ���
    private void CalculateMovement()
    {
        if (isDash)  
        {
            playerSpeed = dashSpeed;
        }
        else { playerSpeed = walkSpeed; }

        Vector3 move = Quaternion.Euler(0, cameraHolder.rotation.eulerAngles.y, 0) * new Vector3(inputMoveVec.x, playerJumpVec, inputMoveVec.y);

        //Vector3 move = new Vector3(inputMoveVec.x, jumpVec, inputMoveVec.y);
        characterController.Move(move * Time.deltaTime * playerSpeed);
        playerJumpVec += gravity * Time.deltaTime;                             // �߷� ����
    }
    // �÷��̾��� ���� ���
    private void CalculateView()
    {
        newCharacterRotation.y += playerSettings.viewYSensitivity * (playerSettings.viewXInverted ? -inputViewVec.x : inputViewVec.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharacterRotation);

        newCameraRotation.x += playerSettings.viewYSensitivity * (playerSettings.viewYInverted ? inputViewVec.y : -inputViewVec.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, playerSettings.viewClampYMin, playerSettings.viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
        lookTarget.localRotation = Quaternion.Euler(newCameraRotation);
        gunPosition.localRotation = Quaternion.Euler(newCameraRotation);

    }

    // �÷��̾� �̵� �Է�
    public void OnMove(InputValue value)
    {
        inputMoveVec = value.Get<Vector2>();
        if (inputMoveVec.y <= 0.7) isDash = false;
    }
    // �÷��̾� �� ���콺 ��Ÿ�� �Է�
    public void OnView(InputValue value)
    {
        inputViewVec = value.Get<Vector2>();
    }
    // ���� �е�� ������ ���
    public void OnViewPad(InputValue value)
    {
        inputViewVec = value.Get<Vector2>();
    }
    // ���� �Է�
    public void OnJump()
    {
        if (characterController.isGrounded) // ������������ ����
        {
            playerJumpVec = jumpForce;
        }
    }
    // ��� �Է�
    public void OnDash(InputValue value)
    {
        dash = value.Get<float>();
        if (dash != 0 && inputMoveVec.y >= 0.7f)  // ��� �����̰� ������ �� ���� ���
        {
            isDash = true;
        }
    }

    // �ִϸ��̼�
    public void ActiveAnimation()
    {
        //// �ȱ� �ִϸ��̼� ����
        if (inputMoveVec.x != 0 || inputMoveVec.y != 0)
        {
            tpsAnimator.SetBool("isWalk", true);
            fpsAnimator.SetBool("isWalk", true);
        }
        else
        {
            tpsAnimator.SetBool("isWalk", false);
            fpsAnimator.SetBool("isWalk", false);
        }
        tpsAnimator.SetBool("isRun", isDash);
        fpsAnimator.SetBool("isRun", isDash);

        if(!characterController.isGrounded && isGrounded)
        {   isGrounded = false;
            tpsAnimator.SetBool("isGrounded", isGrounded);
            fpsAnimator.SetBool("isGrounded", isGrounded);
        }
        else if (characterController.isGrounded && !isGrounded)
        { 
            isGrounded = true;
            tpsAnimator.SetBool("isGrounded", isGrounded);
            fpsAnimator.SetBool("isGrounded", isGrounded);
        }
        tpsAnimator.SetFloat("xDir", inputMoveVec.x);
        tpsAnimator.SetFloat("yDir", inputMoveVec.y);
    }

}
