using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;
using static PlayerSetting; // 플레이어 모델 셋팅 클래스


public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs input; 
    private CharacterController controller; 
    public Animator tpsAnimator;    // 플레이어 TPS 모델 애니메이터
    public Animator fpsAnimator;    // 플레이어 FPS 모델 애니메이터

    [Header("Player")]
    [Tooltip("플레이어 이동속도 m/s")]
    public float moveSpeed;
    [Tooltip("플레이어 대시속도 m/s")]
    public float dashSpeed;
    [Tooltip("플레이어 회전속도")]
    public float rotationSpeed;
    [Tooltip("이동 가속도")]
    public float speedChangeRate;

    [Space(10)]
    [Tooltip("점프 높이")]
    public float jumpHeight;
    [Tooltip("플레이어 중력 값. 중력 기본값 : -9.81f")]
    public float gravity;

    [Space(10)]
    [Tooltip("점프 상태를 확인하는 값. 값이 0이면 바로 점프 가능")]
    public float jumpTimeout;
    [Tooltip("추락 상태를 확인하는 값")]
    public float fallTimeout;

    [Header("Player Grounded")]
    [Tooltip("바닥에 있는지 없는지 체크")]
    public bool isGrounded = true;
    [Tooltip("바닥의 오차")]
    public float groundedOffset;
    [Tooltip("바닥 체크 영역")]
    public float groundedRadius;


    [Header("Cinemachine")]
    [Tooltip("시네버신 버츄얼 카메라가 따라갈 타겟. FPS 플레이어의 머리 위치")]
    public GameObject cinemachineCameraTarget;
    [Tooltip("카메라 최대 각도")]
    public float topClamp;
    [Tooltip("카메라 최소 각도")]
    public float bottomClamp;

    // cinemachine
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private const float _threshold = 0.01f;
    private bool IsCurrentDeviceMouse;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInputs>();
        controller = GetComponent<CharacterController>();
        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
    }

    // Update is called once per frame
    void Update()
    {
        GroundedCheck();    // 바닥 체크
        JumpAndGravity();   // 점프와 중력 관련 메서드
        Move();             // 이동 관련 메서드
        ActiveAnimation();  // 애니메이션 적용
    }
    private void LateUpdate()
    {
        CameraRotation();
    }
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        isGrounded = controller.isGrounded;
    }
    private void CameraRotation()
    {
        // if there is an input
        if (input.look.sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += input.look.y * rotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = input.look.x * rotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            // Update Cinemachine camera target pitch
            cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }
    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = input.dash ? dashSpeed : moveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (input.move != Vector2.zero)
        {
            // move
            inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
        }

        // move the player
        controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }
    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = fallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = jumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
    }

    //// 애니메이션
    public void ActiveAnimation()
    {
        //// 걷기 애니메이션 셋팅
        if (input.move.x != 0 || input.move.y != 0)
        {
            tpsAnimator.SetBool("isWalk", true);
            fpsAnimator.SetBool("isWalk", true);
        }
        else
        {
            tpsAnimator.SetBool("isWalk", false);
            fpsAnimator.SetBool("isWalk", false);
        }
        tpsAnimator.SetBool("isRun", input.dash);
        fpsAnimator.SetBool("isRun", input.dash);

        if (isGrounded)
        {
            tpsAnimator.SetBool("isGrounded", isGrounded);
            fpsAnimator.SetBool("isGrounded", isGrounded);
        }
        else if (!isGrounded)
        {
            tpsAnimator.SetBool("isGrounded", isGrounded);
            fpsAnimator.SetBool("isGrounded", isGrounded);
        }
        tpsAnimator.SetFloat("xDir", input.move.x);
        tpsAnimator.SetFloat("yDir", input.move.y);
    }

}
