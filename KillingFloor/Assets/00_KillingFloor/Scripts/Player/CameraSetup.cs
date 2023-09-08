using Cinemachine; // �ó׸ӽ� ���� �ڵ�
using UnityEngine;

// �ó׸ӽ� ī�޶� ���� �÷��̾ �����ϵ��� ����
public class CameraSetup : MonoBehaviour
{
    GameObject fpsCam;
    GameObject tpsCam;
    public CinemachineVirtualCamera followCam; // ���� ī�޶�
    public GameObject tpsPlayerBody;
    public GameObject fpsPlayerBody;
    public bool isFPS;

    void Awake()
    {
            // ���� �ִ� �ó� �ӽ� ���� ī�޶� ã�� �÷��̾� ������ �ֱ�
            tpsCam = GameObject.FindWithTag("TPS CAM");
            tpsCam.transform.parent = this.transform;
            tpsCam.SetActive(false);                    // 3��Ī�� �̸� ���α�
            tpsPlayerBody.SetActive(false);


            fpsCam = GameObject.FindWithTag("FPS CAM");
            fpsCam.transform.parent = this.transform;

            followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();
            isFPS = true;

            ChangeCamera(followCam);
    }

    // I ��ư�� ������ ī�޶� ����
    public void OnChangeCamera()
    {
            if (isFPS) // 1��Ī�� ��
            {
                fpsCam.SetActive(false);                    
                tpsCam.SetActive(true);                  
                tpsPlayerBody.SetActive(true);
                fpsPlayerBody.SetActive(false);

                followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();

                ChangeCamera(followCam);
                isFPS = false;
            }
            else if (!isFPS) // 3��Ī�� ��
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
    // ī�޶� ����
    public void ChangeCamera(CinemachineVirtualCamera _followCam)
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();

        // ���� ī�޶��� ���� ����� �ڽ��� Ʈ���������� ����
        _followCam.Follow = playerMovement.cinemachineCameraTarget.transform;
        if (isFPS)
        { _followCam.LookAt = playerMovement.cinemachineCameraTarget.transform; }
        else
            _followCam.LookAt = null;

        _followCam.transform.parent = this.transform;

    }
}