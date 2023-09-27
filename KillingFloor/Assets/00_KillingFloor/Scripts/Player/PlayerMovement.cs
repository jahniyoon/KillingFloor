using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviourPun
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
    public CinemachineVirtualCamera followCamera;
    [Tooltip("카메라 최대 각도")]
    public float topClamp;
    [Tooltip("카메라 최소 각도")]
    public float bottomClamp;

    public Transform weaponTarget;



    // cinemachine
    private float _cinemachineTargetPitch;

    // player
    public float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private const float _threshold = 0.01f;
    private bool IsCurrentDeviceMouse;

    private float animMoveSpeed;
    private AudioSource m_AudioSource;
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
    private float m_StepCycle;
    private float m_NextStep;

    //SSM 230925
    public GameObject shopNav;
    private List<GameObject> shopNavList;
    private GameObject shopNavPrant;
    private bool NavAct = false;

    private List<string> cashTem;
   
    //SSM End

    void Start()
    {
        
        cashTem = new List<string>();
        //ssm
        for (int i = 0; i < GameManager.instance.cashItem.Count; i++)
        {
            if (GameManager.instance.cashItem[i] == "GoldSkin")
            {
                
                photonView.RPC("changeMaster", RpcTarget.MasterClient,photonView.ViewID);   
             
            }
        }

        shopNavList = new List<GameObject>();
        shopNavPrant = GameObject.Find("ShopNavigation");
        if (shopNavPrant != null)
        {
            for (int i = 0; i < 30; i++)
            {
                GameObject saveObj = Instantiate(shopNav, shopNavPrant.transform);
                shopNavList.Add(saveObj);
                shopNavList[i].SetActive(false);
            }
        }

        //ssm end
        input = GetComponent<PlayerInputs>();
        controller = GetComponent<CharacterController>();
        m_AudioSource = GetComponent<AudioSource>();
        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
    }
    public void OnEnable()
    {
        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
    }
    void Update()
    {
        // 입력 가능여부 확인
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.
        MouseSensitiveUpdate();
        GroundedCheck();    // 바닥 체크
        JumpAndGravity();   // 점프와 중력 관련 메서드
        Move();             // 이동 관련 메서드
        ActiveAnimation();  // 애니메이션 적용
        ShopUIUpdate();     // 이동에 따라 상점 UI 업데이트

        // 입력 가능 여부 확인
        if (GameManager.instance.inputLock)
        {
            GetComponent<PlayerInput>().enabled = false;
        }
        else if (!GameManager.instance.inputLock)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
        if (GameManager.instance.isShop == true)
        {
            photonView.RPC("shopNavSp", RpcTarget.All);
       
        }
    }

    private void LateUpdate()
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.
        CameraRotation();
    }

    public void MouseSensitiveUpdate()
    {
        rotationSpeed = PlayerUIManager.instance.mouseSensitive.value;
    }
    // 바닥 체크
    private void GroundedCheck()
    {
        isGrounded = controller.isGrounded;
    }
    private void JumpAndGravity()
    {
        if (GameManager.instance != null && GameManager.instance.inputLock)
            return;

        if (isGrounded)
        {
            // 추락 타임아웃 초기화
            _fallTimeoutDelta = fallTimeout;

            tpsAnimator.SetBool("isJump", false);
            tpsAnimator.SetBool("isAir", false);

            // 수직 힘이 0보다 떨어졌을 경우 -2로 제한시켜 속도가 무한하게 떨어지는 것 방지
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // 점프
            if (input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // H * -2 * G 의 제곱근 = 원하는 높이에 도달하는데 필요한 속도
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                tpsAnimator.SetBool("isJump", true);

            }

            // 점프 타임아웃
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // 바닥에 닿아있는 상태라면 점프 타임아웃 초기화
            _jumpTimeoutDelta = jumpTimeout;

            // 낙하 상태 타임아웃도 초기화
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            else
                tpsAnimator.SetBool("isAir", true);

            // 바닥이 아니면 점프를 못하게 처리
            input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }
    private void CameraRotation()
    {
        if (GameManager.instance != null && GameManager.instance.inputLock)
            return;
        // 마우스 입력이 있으면
        if (input.look.sqrMagnitude >= _threshold)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += input.look.y * rotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = input.look.x * rotationSpeed * deltaTimeMultiplier;

            // 회전값 제한
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            // 시네머신 카메라 타겟 업데이트
            cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
            weaponTarget.localRotation = Quaternion.Euler(_cinemachineTargetPitch * -1, 0.0f, 0.0f);    // 모델링이 반대로 값을 받아서 -1을 곱함
            followCamera.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);   // 시네머신 또한 위아래 회전 가능하게 설정. (안하면 고정되어있음)

            // 플레이어 좌우로 회전시키기
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }
    private void Move()
    {
        // 타겟 속도를 대시 상태에 따라 달라지게 설정
        //float targetSpeed = input.dash ? dashSpeed : moveSpeed;
        float targetSpeed = moveSpeed;
        if (input.dash & 0.7 <= input.move.y)
        { targetSpeed = dashSpeed; }

        if (GameManager.instance != null && GameManager.instance.inputLock)
        { input.move = Vector2.zero; }

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // Vector2의 ==연산자는 근사치를 사용하므로, 부동소수점 오류가 발생하지 않는다.
        // 입력이 없으면 속도는 0으로 제한
        if (input.move == Vector2.zero) targetSpeed = 0.0f;

        // 플레이어의 수평 속도
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = input.move.magnitude;

        // 목표 속도까지 가속 또는 감속을 한다
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // 보다 유기적인 속도 변화를 위해 선형이 아닌 곡선 결과로 생성. 보간?
            // Lerp의 T가 고정되어있으므로 속도를 따로 고정할 필요가 없다고한다. T는 Time.deltaTime * speedChangeRate
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            // 속도를 소수점 이하 3자리로 반올림
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }
        // 플레이어 이동 애니메이션 블랜드 속도
        animMoveSpeed = Mathf.Lerp(animMoveSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animMoveSpeed < 0.01f) animMoveSpeed = 0f;


        // 입력 방향 정규화
        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        // move 입력이 있는 경우 플레이어가 움직일 때 플레이어 회전
        if (input.move != Vector2.zero)
        {
            // 이동
            inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
        }


        // 플레이어 위치 이동
        controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        ProgressStepCycle(moveSpeed);

    }
    // 발자국 소리
    private void ProgressStepCycle(float speed)
    {
        if (controller.velocity.sqrMagnitude > 0 && (input.move.x != 0 || input.move.y != 0))
        {
            m_StepCycle += (controller.velocity.magnitude + (speed * (input.dash ? 0.3f : 0))) *
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + 5;

        PlayFootStepAudio();
    }
    // 발자국 소리 재생
    private void PlayFootStepAudio()
    {
        if (!controller.isGrounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }
    // 카메라 각도 제한
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    // 바닥과 닿았는지 확인할 수 있는 기즈모
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // 기즈모 그리기
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
    }

    //// 애니메이션
    public void ActiveAnimation()
    {
        if (fpsAnimator != null)
        {
            //// 걷기 애니메이션 셋팅
            if (input.move.x != 0 || input.move.y != 0)
            {
                fpsAnimator.SetBool("isWalk", true);
            }
            else
            {
                fpsAnimator.SetBool("isWalk", false);
            }
            fpsAnimator.SetBool("isRun", input.dash);
            if (isGrounded)
            {
                fpsAnimator.SetBool("isGrounded", isGrounded);
            }
            else if (!isGrounded)
            {
                fpsAnimator.SetBool("isGrounded", isGrounded);
            }
        }
        tpsAnimator.SetBool("isJump", input.jump);

        if (isGrounded)
        {
            tpsAnimator.SetBool("isGrounded", isGrounded);
        }
        else if (!isGrounded)
        {
            tpsAnimator.SetBool("isGrounded", isGrounded);
        }

        tpsAnimator.SetFloat("Speed", animMoveSpeed);
    }
    // 상점 UI 업데이트
    public void ShopUIUpdate()
    {

        // 상점과의 거리 계산하기
        float shopDistance = Mathf.FloorToInt(Vector3.Distance(controller.transform.position, GameManager.instance.shopPosition.position));
        PlayerUIManager.instance.SetShopDistance(shopDistance); // 상점 거리 업데이트
        // 플레이어의 현재방향에서 상점까지의 벡터간 각도 계산하기
        Vector3 playerForward = controller.transform.rotation * Vector3.forward;
        float shopAngle = Vector3.SignedAngle(playerForward, GameManager.instance.shopPosition.position - controller.transform.position, Vector3.up);

        if (GameManager.instance.shopPosition.position.y + 1f > controller.transform.position.y)
        { PlayerUIManager.instance.SetShopRotation(shopAngle, true); }
        else
            PlayerUIManager.instance.SetShopRotation(shopAngle, false);


    }
    [PunRPC]
    public void shopNavSp()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        //SSM 20230925 네비 소환
        if (NavAct== false)
        {
            for (int i = 0; i < shopNavList.Count; i++)
            {
                if (!shopNavList[i].activeSelf)
                {
                    shopNavList[i].transform.position = transform.position;
                    shopNavList[i].SetActive(true);
                    NavAct = true;
                    StartCoroutine(shopNavSpTime());
                    break;
                }
            }
        }

        //SSM End
    }
    private IEnumerator shopNavSpTime()
    {
        yield return new WaitForSeconds(3);
        NavAct = false;


    }
    [PunRPC]
    private void changeMaster(int viewid)
    {

        photonView.RPC("changeRender", RpcTarget.All, viewid);
    
    }
    [PunRPC]
    private void changeRender(int viewid)
    {
        GameObject user = PhotonView.Find(viewid).gameObject;
        Transform childObject = user.transform.Find("genAssault_LOD0");
        Renderer renderer = childObject.GetComponent<Renderer>();
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = Color.blue;
        renderer.material = newMaterial;
    }
}
