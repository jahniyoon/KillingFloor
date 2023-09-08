using Cinemachine; // 시네머신 관련 코드
using UnityEngine;

// 시네머신 카메라가 로컬 플레이어를 추적하도록 설정
public class CameraSetup : MonoBehaviour
{
    GameObject fpsCam;
    GameObject tpsCam;
    public CinemachineVirtualCamera followCam; // 현재 카메라
    public GameObject tpsPlayerBody;
    public GameObject fpsPlayerBody;
    public bool isFPS;

    void Awake()
    {
            // 씬에 있는 시네 머신 가상 카메라를 찾고 플레이어 하위에 넣기
            tpsCam = GameObject.FindWithTag("TPS CAM");
            tpsCam.transform.parent = this.transform;
            tpsCam.SetActive(false);                    // 3인칭은 미리 꺼두기
            tpsPlayerBody.SetActive(false);


            fpsCam = GameObject.FindWithTag("FPS CAM");
            fpsCam.transform.parent = this.transform;

            followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();
            isFPS = true;

            ChangeCamera(followCam);
    }

    // I 버튼을 누르면 카메라 변경
    public void OnChangeCamera()
    {
            if (isFPS) // 1인칭일 때
            {
                fpsCam.SetActive(false);                    
                tpsCam.SetActive(true);                  
                tpsPlayerBody.SetActive(true);
                fpsPlayerBody.SetActive(false);

                followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();

                ChangeCamera(followCam);
                isFPS = false;
            }
            else if (!isFPS) // 3인칭일 때
            {
                tpsCam.SetActive(false);
                fpsCam.SetActive(true);
                tpsPlayerBody.SetActive(false);
                fpsPlayerBody.SetActive(true);
    
                followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();

                ChangeCamera(followCam);
                isFPS = true;
            }
    }
    // 카메라 변경
    public void ChangeCamera(CinemachineVirtualCamera _followCam)
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();

        // 가상 카메라의 추적 대상을 자신의 트랜스폼으로 변경
        _followCam.Follow = playerMovement.cinemachineCameraTarget.transform;
        if (isFPS)
        { _followCam.LookAt = playerMovement.cinemachineCameraTarget.transform; }
        else
            _followCam.LookAt = null;

        _followCam.transform.parent = this.transform;

    }
}