using Cinemachine; // 시네머신 관련 코드
using UnityEngine;

// 시네머신 카메라가 로컬 플레이어를 추적하도록 설정
public class CameraSetup : MonoBehaviour
{
    GameObject fpsCam;
    GameObject tpsCam;
    CinemachineVirtualCamera followCam; // 현재 카메라
    
    public GameObject fpsBody;  // FPS 추적할 대상
    public GameObject tpsBody;  // TPS 추적할 대상

    bool isFPS;
    void Awake()
    {
            // 씬에 있는 시네 머신 가상 카메라를 찾고 플레이어 하위에 넣기
            tpsCam = GameObject.FindWithTag("TPS CAM");
            tpsCam.transform.parent = this.transform;
            tpsCam.SetActive(false);                    // 3인칭은 미리 꺼두기
            tpsBody.SetActive(false);

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
                fpsBody.SetActive(false);

                tpsCam.SetActive(true);
                tpsBody.SetActive(true);


                followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();
                isFPS = false;

                ChangeCamera(followCam);
            }
            else if (!isFPS) // 3인칭일 때
            {
                tpsCam.SetActive(false);
                tpsBody.SetActive(false);

                fpsCam.SetActive(true);
                fpsBody.SetActive(true);

                followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();
                isFPS = true;

                ChangeCamera(followCam);
            }
    }
    // 카메라 변경
    public void ChangeCamera(CinemachineVirtualCamera _followCam)
    {
        // 가상 카메라의 추적 대상을 자신의 트랜스폼으로 변경
        _followCam.Follow = transform;
        _followCam.transform.parent = this.transform;

        // 플레이어에게 달아주기
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        playerMovement.cameraHolder = _followCam.transform;
    }
}