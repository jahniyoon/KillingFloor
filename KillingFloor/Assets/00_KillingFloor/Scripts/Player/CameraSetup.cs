using Cinemachine; // 시네머신 관련 코드
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

// 시네머신 카메라가 로컬 플레이어를 추적하도록 설정
public class CameraSetup : MonoBehaviourPun
{
    PlayerInputs input;
    PlayerMovement playerMovement;
    GameObject fpsCam;
    GameObject tpsCam;
    GameObject inspectorCam;
    public CinemachineVirtualCamera followCam; // 현재 카메라
    public GameObject tpsPlayerBody;    // 3인칭 플레이어 바디
    public GameObject fpsPlayerBody;    // 1인칭 플레이어 바디
    public GameObject playerSpine;
    public bool isFPS;
    public bool tpsTest;

    void Awake()
    {
        if (photonView.IsMine)
        {
            // 카메라 셋 하면서 화면 락 걸어주기
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            playerMovement = GetComponent<PlayerMovement>();
            input = GetComponent<PlayerInputs>();

            // 씬에 있는 시네 머신 가상 카메라를 찾고 플레이어 하위에 넣기
            tpsCam = GameObject.FindWithTag("TPS CAM");
            tpsCam.transform.parent = this.transform;
            fpsCam = GameObject.FindWithTag("FPS CAM");
            fpsCam.transform.parent = this.transform;
            inspectorCam = GameObject.FindWithTag("Inspector CAM");
            playerSpine = this.transform.GetChild(1).gameObject;
            playerSpine.SetActive(false);               // 3인칭 총도 꺼주기
            tpsCam.SetActive(false);                    // 3인칭은 미리 꺼두기 (Debug용)
            inspectorCam.SetActive(false);

            followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();    // FPS 카메라를 팔로우캠으로 설정
            playerMovement.followCamera = followCam;
            CameraSet(followCam);   // 카메라 세팅
            if (tpsTest)
            {
                TPSTest();
                Destroy(fpsPlayerBody);
            }
        }
            fpsPlayerBody.SetActive(photonView.IsMine);
            tpsPlayerBody.SetActive(!photonView.IsMine);
        
    }

    void Update()
    {
        ChangeCamera();
    }

    public void ChangeCamera()
    {
        if (photonView.IsMine)
        {
            if (input.changeCamera) // 버튼이 눌리면 실행
            {
                SetCamera();
                input.changeCamera = false; // 카메라가 변경되면 다시 입력 가능
            }
        }
    }

    public void SetCamera()
    {
        inspectorCam.SetActive(false);
        tpsCam.SetActive(isFPS);
        fpsCam.SetActive(!isFPS);
        tpsPlayerBody.SetActive(isFPS);
        fpsPlayerBody.SetActive(!isFPS);
        playerSpine.SetActive(isFPS);


        if (isFPS) // 1인칭일 때
        {
            followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();
        }
        else if (!isFPS) // 3인칭일 때
        {
            followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();
        }
        CameraSet(followCam);
    }

    public void InspectorCam(GameObject _player)
    {
        tpsCam.SetActive(false);
        fpsCam.SetActive(false);
        tpsPlayerBody.SetActive(true);
        fpsPlayerBody.SetActive(false);

        inspectorCam.SetActive(true);
        followCam = inspectorCam.GetComponent<CinemachineVirtualCamera>();
        followCam.Follow = _player.GetComponent<PlayerMovement>().cinemachineCameraTarget.transform;
        followCam.LookAt = _player.GetComponent<PlayerMovement>().cinemachineCameraTarget.transform;

    }
    public void TPSTest()
    {
        tpsCam.SetActive(isFPS);
        fpsCam.SetActive(!isFPS);
        tpsPlayerBody.SetActive(isFPS);
        fpsPlayerBody.SetActive(!isFPS);

        if (isFPS) // 1인칭일 때
        {
            followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();
        }
        else if (!isFPS) // 3인칭일 때
        {
            followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();
        }
        CameraSet(followCam);
        input.changeCamera = false; // 카메라가 변경되면 다시 입력 가능
    }
    // 카메라 변경
    public void CameraSet(CinemachineVirtualCamera _followCam)
    {
        isFPS = !isFPS; // 스스로 변경

        // 가상 카메라의 추적 대상을 자신의 트랜스폼으로 변경
        _followCam.Follow = playerMovement.cinemachineCameraTarget.transform;

        if (!isFPS)  // 만약 FPS면 LookAt도 추가
        { _followCam.LookAt = playerMovement.cinemachineCameraTarget.transform; }
        else
        _followCam.LookAt = null;

    }
}