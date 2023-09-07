using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;
using static PlayerSetting; // �÷��̾� �� ���� Ŭ����


public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs input; 
    private CharacterController controller; 
    public Animator tpsAnimator;    // �÷��̾� TPS �� �ִϸ�����
    public Animator fpsAnimator;    // �÷��̾� FPS �� �ִϸ�����

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed;
    [Tooltip("Rotation speed of the character")]
    public float RotationSpeed;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp;

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


    //[Header("Settings")]
    //public PlayerSettingsModel playerSettings;

    //private Vector3 newCameraRotation;
    //private Vector3 newCharacterRotation;
    //public Transform cameraHolder; // ī�޶� ���� �� (�÷��̾ ���� ���⿡ �°� ī�޶� ���� ����)
    //public Transform lookTarget; // ī�޶� ���� �� (�÷��̾ ���� ���⿡ �°� ī�޶� ���� ����)
    //public Transform gunPosition;

    //[Header("Player")]
    //public Vector2 inputMoveVec;
    //public Vector2 inputViewVec;

    //private float playerJumpVec;      // ���� ���� ��
    //private float playerSpeed;  // �÷��̾��� ���� �ӵ�

    //public float walkSpeed;     // �÷��̾� �ȴ� �ӵ�
    //public float dashSpeed;     // �÷��̾� ��� �ӵ�
    //public float rotaionSpeed;  // ȸ�� �ӵ�
    //public float jumpForce;     // ���� ��
    //public float gravity;       // �߷°�

    //private float dash;         // ��ù�ư �Է� Ȯ��
    //private bool isDash;        // ��� �����ΰ�
    //private bool isGrounded = true;    // �ٴ��ΰ�
    //public bool isDie;          // �׾��� ToDo : ���� ��ƼƼ�� �̵�


    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInputs>();
        controller = GetComponent<CharacterController>();
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    // Update is called once per frame
    void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
        //Cursor.lockState = CursorLockMode.Locked;   // ���콺 ����

        //if (!isDie)
        //{
        //    CalculateMovement();    // �÷��̾� �̵� ���
        //    CalculateView();        // �÷��̾� ���� ���
        //    ActiveAnimation();      // �ִϸ��̼� ����
        //}
    }
    private void LateUpdate()
    {
        CameraRotation();
    }
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        //Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        Grounded = controller.isGrounded;
    }
    private void CameraRotation()
    {
        // if there is an input
        if (input.look.sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += input.look.y * RotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = input.look.x * RotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }
    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = input.dash ? SprintSpeed : MoveSpeed;

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
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

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
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
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
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            input.jump = false;
            Debug.Log("false�� �Ǳ� �ϳ�?");
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
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

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }




    //// �÷��̾��� �̵� ���
    //private void CalculateMovement()
    //{
    //    if (isDash)  
    //    {
    //        playerSpeed = dashSpeed;
    //    }
    //    else { playerSpeed = walkSpeed; }

    //    Vector3 move = Quaternion.Euler(0, cameraHolder.rotation.eulerAngles.y, 0) * new Vector3(inputMoveVec.x, playerJumpVec, inputMoveVec.y);

    //    //Vector3 move = new Vector3(inputMoveVec.x, jumpVec, inputMoveVec.y);
    //    characterController.Move(move * Time.deltaTime * playerSpeed);
    //    playerJumpVec += gravity * Time.deltaTime;                             // �߷� ����
    //}
    //// �÷��̾��� ���� ���
    //private void CalculateView()
    //{
    //    newCharacterRotation.y += playerSettings.viewYSensitivity * (playerSettings.viewXInverted ? -inputViewVec.x : inputViewVec.x) * Time.deltaTime;
    //    transform.localRotation = Quaternion.Euler(newCharacterRotation);

    //    newCameraRotation.x += playerSettings.viewYSensitivity * (playerSettings.viewYInverted ? inputViewVec.y : -inputViewVec.y) * Time.deltaTime;
    //    newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, playerSettings.viewClampYMin, playerSettings.viewClampYMax);

    //    cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
    //    lookTarget.localRotation = Quaternion.Euler(newCameraRotation);
    //    gunPosition.localRotation = Quaternion.Euler(newCameraRotation);

    //}

    //// �÷��̾� �̵� �Է�
    //public void OnMove(InputValue value)
    //{
    //    inputMoveVec = value.Get<Vector2>();
    //    if (inputMoveVec.y <= 0.7) isDash = false;
    //}
    //// �÷��̾� �� ���콺 ��Ÿ�� �Է�
    //public void OnView(InputValue value)
    //{
    //    inputViewVec = value.Get<Vector2>();
    //}
    //// ���� �е�� ������ ���
    //public void OnViewPad(InputValue value)
    //{
    //    inputViewVec = value.Get<Vector2>();
    //}
    ////���� �Է�
    //public void OnJump()
    //{
    //    if (characterController.isGrounded) // ������������ ����
    //    {
    //        playerJumpVec = jumpForce;
    //    }
    //}
    //// ��� �Է�
    //public void OnDash(InputValue value)
    //{
    //    dash = value.Get<float>();
    //    if (dash != 0 && inputMoveVec.y >= 0.7f)  // ��� �����̰� ������ �� ���� ���
    //    {
    //        isDash = true;
    //    }
    //}

    //// �ִϸ��̼�
    //public void ActiveAnimation()
    //{
    //    //// �ȱ� �ִϸ��̼� ����
    //    if (inputMoveVec.x != 0 || inputMoveVec.y != 0)
    //    {
    //        tpsAnimator.SetBool("isWalk", true);
    //        fpsAnimator.SetBool("isWalk", true);
    //    }
    //    else
    //    {
    //        tpsAnimator.SetBool("isWalk", false);
    //        fpsAnimator.SetBool("isWalk", false);
    //    }
    //    tpsAnimator.SetBool("isRun", isDash);
    //    fpsAnimator.SetBool("isRun", isDash);

    //    if(!characterController.isGrounded && isGrounded)
    //    {   isGrounded = false;
    //        tpsAnimator.SetBool("isGrounded", isGrounded);
    //        fpsAnimator.SetBool("isGrounded", isGrounded);
    //    }
    //    else if (characterController.isGrounded && !isGrounded)
    //    { 
    //        isGrounded = true;
    //        tpsAnimator.SetBool("isGrounded", isGrounded);
    //        fpsAnimator.SetBool("isGrounded", isGrounded);
    //    }
    //    tpsAnimator.SetFloat("xDir", inputMoveVec.x);
    //    tpsAnimator.SetFloat("yDir", inputMoveVec.y);
    //}

}
