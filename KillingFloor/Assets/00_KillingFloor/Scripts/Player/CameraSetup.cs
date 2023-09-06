using Cinemachine; // �ó׸ӽ� ���� �ڵ�
using UnityEngine;

// �ó׸ӽ� ī�޶� ���� �÷��̾ �����ϵ��� ����
public class CameraSetup : MonoBehaviour
{
    GameObject fpsCam;
    GameObject tpsCam;
    CinemachineVirtualCamera followCam; // ���� ī�޶�
    
    public GameObject fpsBody;  // FPS ������ ���
    public GameObject tpsBody;  // TPS ������ ���

    bool isFPS;
    void Awake()
    {
            // ���� �ִ� �ó� �ӽ� ���� ī�޶� ã�� �÷��̾� ������ �ֱ�
            tpsCam = GameObject.FindWithTag("TPS CAM");
            tpsCam.transform.parent = this.transform;
            tpsCam.SetActive(false);                    // 3��Ī�� �̸� ���α�
            tpsBody.SetActive(false);

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
                fpsBody.SetActive(false);

                tpsCam.SetActive(true);
                tpsBody.SetActive(true);


                followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();
                isFPS = false;

                ChangeCamera(followCam);
            }
            else if (!isFPS) // 3��Ī�� ��
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
    // ī�޶� ����
    public void ChangeCamera(CinemachineVirtualCamera _followCam)
    {
        // ���� ī�޶��� ���� ����� �ڽ��� Ʈ���������� ����
        _followCam.Follow = transform;
        _followCam.transform.parent = this.transform;

        // �÷��̾�� �޾��ֱ�
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        playerMovement.cameraHolder = _followCam.transform;
    }
}