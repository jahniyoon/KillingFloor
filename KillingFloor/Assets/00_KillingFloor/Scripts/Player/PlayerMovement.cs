using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerSetting; // 플레이어 모델 셋팅 클래스


public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    public Animator tpsAnimator;
    public Animator fpsAnimator;
    private PlayerAction playerInput;                    // 인풋 시스템 활용시 이름 동일해야함

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;
    public Transform cameraHolder; // 카메라가 담기는 곳 (플레이어가 보는 방향에 맞게 카메라 방향 변경)
    public Transform lookTarget; // 카메라가 담기는 곳 (플레이어가 보는 방향에 맞게 카메라 방향 변경)
    public Transform gunPosition;

    [Header("Player")]
    public Vector2 inputMoveVec;
    public Vector2 inputViewVec;

    private float playerJumpVec;      // 현재 점프 힘
    private float playerSpeed;  // 플레이어의 현재 속도

    public float walkSpeed;     // 플레이어 걷는 속도
    public float dashSpeed;     // 플레이어 대시 속도
    public float rotaionSpeed;  // 회전 속도
    public float jumpForce;     // 점프 힘
    public float gravity;       // 중력값

    private float dash;         // 대시버튼 입력 확인
    private bool isDash;        // 대시 상태인가
    private bool isGrounded = true;    // 바닥인가
    public bool isDie;          // 죽었나 ToDo : 리빙 엔티티로 이동


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Animator tpsAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;   // 마우스 고정

        if (!isDie)
        {
            CalculateMovement();    // 플레이어 이동 계산
            CalculateView();        // 플레이어 시점 계산
            ActiveAnimation();      // 애니메이션 적용
        }
    }
    // 플레이어의 이동 계산
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
        playerJumpVec += gravity * Time.deltaTime;                             // 중력 구현
    }
    // 플레이어의 시점 계산
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

    // 플레이어 이동 입력
    public void OnMove(InputValue value)
    {
        inputMoveVec = value.Get<Vector2>();
        if (inputMoveVec.y <= 0.7) isDash = false;
    }
    // 플레이어 뷰 마우스 델타값 입력
    public void OnView(InputValue value)
    {
        inputViewVec = value.Get<Vector2>();
    }
    // 게임 패드로 적용할 경우
    public void OnViewPad(InputValue value)
    {
        inputViewVec = value.Get<Vector2>();
    }
    // 점프 입력
    public void OnJump()
    {
        if (characterController.isGrounded) // 땅에있을때만 점프
        {
            playerJumpVec = jumpForce;
        }
    }
    // 대시 입력
    public void OnDash(InputValue value)
    {
        dash = value.Get<float>();
        if (dash != 0 && inputMoveVec.y >= 0.7f)  // 대시 상태이고 앞으로 갈 때만 대시
        {
            isDash = true;
        }
    }

    // 애니메이션
    public void ActiveAnimation()
    {
        //// 걷기 애니메이션 셋팅
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
