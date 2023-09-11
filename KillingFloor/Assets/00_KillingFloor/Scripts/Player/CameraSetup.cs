using Cinemachine; // �ó׸ӽ� ���� �ڵ�
using UnityEngine;

// �ó׸ӽ� ī�޶� ���� �÷��̾ �����ϵ��� ����
public class CameraSetup : MonoBehaviour
{
    PlayerInputs input;
    PlayerMovement playerMovement;
    GameObject fpsCam;
    GameObject tpsCam;
    public CinemachineVirtualCamera followCam; // ���� ī�޶�
    public GameObject tpsPlayerBody;    // 3��Ī �÷��̾� �ٵ�
    public GameObject fpsPlayerBody;    // 1��Ī �÷��̾� �ٵ�
    public bool isFPS;
    public bool tpsTest;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInputs>();

        // ���� �ִ� �ó� �ӽ� ���� ī�޶� ã�� �÷��̾� ������ �ֱ�
        tpsCam = GameObject.FindWithTag("TPS CAM");
        tpsCam.transform.parent = this.transform;
        fpsCam = GameObject.FindWithTag("FPS CAM");
        fpsCam.transform.parent = this.transform;

        tpsCam.SetActive(false);                    // 3��Ī�� �̸� ���α� (Debug��)
        tpsPlayerBody.SetActive(false);


        followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();    // FPS ī�޶� �ȷο�ķ���� ����
        playerMovement.followCamera = followCam;
        CameraSet(followCam);   // ī�޶� ����
        if (tpsTest){ TPSTest();
            Destroy(fpsPlayerBody);
        }
    }

    void Update()
    {
        ChangeCamera();
    }

    public void ChangeCamera()
    {
        if (input.changeCamera) // ��ư�� ������ ����
        {
            tpsCam.SetActive(isFPS);
            fpsCam.SetActive(!isFPS);
            tpsPlayerBody.SetActive(isFPS);
            fpsPlayerBody.SetActive(!isFPS);

            if (isFPS) // 1��Ī�� ��
            {
                followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();
            }
            else if (!isFPS) // 3��Ī�� ��
            {
                followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();
            }
            CameraSet(followCam);
            input.changeCamera = false; // ī�޶� ����Ǹ� �ٽ� �Է� ����
        }
    }

    public void TPSTest()
    {
        tpsCam.SetActive(isFPS);
        fpsCam.SetActive(!isFPS);
        tpsPlayerBody.SetActive(isFPS);
        fpsPlayerBody.SetActive(!isFPS);

        if (isFPS) // 1��Ī�� ��
        {
            followCam = tpsCam.GetComponent<CinemachineVirtualCamera>();
        }
        else if (!isFPS) // 3��Ī�� ��
        {
            followCam = fpsCam.GetComponent<CinemachineVirtualCamera>();
        }
        CameraSet(followCam);
        input.changeCamera = false; // ī�޶� ����Ǹ� �ٽ� �Է� ����
    }
    // ī�޶� ����
    public void CameraSet(CinemachineVirtualCamera _followCam)
    {
        isFPS = !isFPS; // ������ ����

        // ���� ī�޶��� ���� ����� �ڽ��� Ʈ���������� ����
        _followCam.Follow = playerMovement.cinemachineCameraTarget.transform;

        if (!isFPS)  // ���� FPS�� LookAt�� �߰�
        { _followCam.LookAt = playerMovement.cinemachineCameraTarget.transform; }
        else
        _followCam.LookAt = null;

    }
}