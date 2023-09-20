using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<GameManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }
    private static GameManager m_instance; // �̱����� �Ҵ�� static ����

    public GameObject playerPrefab; // ������ �÷��̾� ĳ���� ������
    public Vector3 spawnPosition;
    public bool isGameover { get; private set; } // ���� ���� ����
    public bool inputEnable = true;

    // ��ȯ �߰�

    [Header("Game Setting")]

    // Junoh �߰�
    public NoticeController noticeController;
    public Volume volume;

    public int round = 1;               // ���� ����
    public int wave = 1;                // ���� ���̺�
    public int player = 4;              // �÷��̾� �ο� ��
    public int difficulty = 0;          // ���̵� 0: ���� 1: ����� 2: ����
    public int currentZombieCount = 0;  // ���� ���� ��
    public bool isZedTime = false;      // ���� Ÿ��
    public bool isSpawnZombie = false;  // ���� ��ȯ ����� Ȯ��
    public bool isCheck = true;         // ���� ���̺갡 ���� Ȯ��
    public bool isZedTimeCheck = false;
    // Junoh �߰�

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
        // ������ ���� ��ġ ����
        //spawnPosition = new Vector3(0f, 1f, 0f);

        if (SceneManager.GetActiveScene().name == "Main")
        {
            Debug.Log("���ξ� ����");
            //spawnPosition = new Vector3(-23.7f, -5.9f, 22.5f);

        }

        // ��Ʈ��ũ ���� ��� Ŭ���̾�Ʈ�鿡�� ���� ����
        // ��, �ش� ���� ������Ʈ�� �ֵ�����, ���� �޼��带 ���� ������ Ŭ���̾�Ʈ���� ����
        //PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0.0f,0.0f,0.0f), Quaternion.identity);
        newPlayer.transform.SetParent(GameObject.Find("Players").transform);
        Debug.Log(newPlayer.transform.position);
    }
    //private void Awake()
    //{
    //    // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
    //    if (instance != this)
    //    {
    //        // �ڽ��� �ı�
    //        Destroy(gameObject);
    //    }
    //    if (instance == null)
    //    { instance = this; }
    //    else
    //    { GlobalFunc.LogWarning("���� �� �� �̻��� ���� �Ŵ����� �����մϴ�."); }
    //}

    // Start is called before the first frame update
    void Start()
    {
      
        // ToDO : �׽�Ʈ������ �Ѿ���� �����ǵ��� �����ϱ�
     
        StartCoroutine(StartWave());
    }

    // Ű���� �Է��� �����ϰ� ���� ������ ��
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PhotonNetwork.LeaveRoom();
        }
        if (isZedTime) { StartCoroutine(ZedTime()); Debug.Log("��� ȣ�� �ϴ°�?"); }
    }

    private IEnumerator ZedTime()
    {
        isZedTime = false;

        float timeElapsed = 0.0f;

        if (isZedTimeCheck == false)
        {
            while (timeElapsed < 0.5f)
            {
                timeElapsed += Time.deltaTime;

                float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / 0.5f), 2);

                Time.timeScale = Mathf.Lerp(1.0f, 0.2f, time);
                volume.weight = Mathf.Lerp(0.0f, 1.0f, time);

                yield return null;
            }

            isZedTimeCheck = true;
        }
        Debug.Log("��ŸƮ");
        timeElapsed = 0.0f;

        while (timeElapsed < 6.0 * 0.2f)
        {
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        Debug.Log("�߰�");

        timeElapsed = 0.0f;

        while (timeElapsed < 0.5f)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / 0.5f), 2);

            Time.timeScale = Mathf.Lerp(0.2f, 1.0f, time);
            volume.weight = Mathf.Lerp(1.0f, 0.0f, time);

            yield return null;
        Debug.Log("������");
        }

        isZedTimeCheck = false;
    }
    // ���� ������ �ڵ� ����Ǵ� �޼���
    public override void OnLeftRoom()
    {
        // ���� ������ �κ� ������ ���ư�
        SceneManager.LoadScene("LoginScene");
    }


    // �ֱ������� �ڵ� ����Ǵ�, ����ȭ �޼���
    // ToDo : ���� ���̺�, ���� ī��Ʈ �� ������Ʈ �ʿ��ϸ� ��Ź�ؿ�
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ���� ������Ʈ��� ���� �κ��� �����
        if (stream.IsWriting)
        {
            // ��Ʈ��ũ�� ���� round ���� ������
            //stream.SendNext(currentZombieCount);
        }
        else
        {
            // ����Ʈ ������Ʈ��� �б� �κ��� �����         

            // ��Ʈ��ũ�� ���� score �� �ޱ�
            //currentZombieCount = (int)stream.ReceiveNext();
            // ����ȭ�Ͽ� ���� ������ UI�� ǥ��
        }

    }

    

   

    // Junoh �߰�
    public void PlusCount(int _num)
    {
        currentZombieCount += _num;
    }

    public void MinusCount(int _num)
    {
        currentZombieCount -= _num;
    }

    public void SetWave(int _num)
    {
        wave += _num;
    }

    private IEnumerator StartWave()
    {
        PlayerUIManager.instance.CountUI.SetActive(true);
        PlayerUIManager.instance.TimerUI.SetActive(false);

        PlayerUIManager.instance.zombieCountText.gameObject.SetActive(true);
        PlayerUIManager.instance.timerCountText.gameObject.SetActive(false);

        while (true)
        {
            if (currentZombieCount > 0) { break; }

            yield return null;
        }

        PlayerUIManager.instance.SetNotice("Start Wave");
        StartCoroutine(noticeController.CoroutineManager(false));

        while (0 < currentZombieCount)
        {
            PlayerUIManager.instance.SetZombieCount(currentZombieCount);

            yield return null;
        }

        StartCoroutine(ChangeWave());
    }

    private IEnumerator ChangeWave()
    {
        PlayerUIManager.instance.SetNotice("Wave Clear");
        PlayerUIManager.instance.SetNoticeLogo("Go to Shop");

        PlayerUIManager.instance.CountUI.SetActive(false);
        PlayerUIManager.instance.TimerUI.SetActive(true);

        PlayerUIManager.instance.zombieCountText.gameObject.SetActive(false);
        PlayerUIManager.instance.timerCountText.gameObject.SetActive(true);

        StartCoroutine(noticeController.CoroutineManager(true));

        int timeElapsed = 10;


        while (0 < timeElapsed)
        {
            timeElapsed -= 1;

            PlayerUIManager.instance.SetTimerCount(timeElapsed);

            yield return new WaitForSeconds(1);
        }

        SetWave(1);
        isCheck = true;
        StartCoroutine(StartWave());
    }
    // Junoh �߰�
}
