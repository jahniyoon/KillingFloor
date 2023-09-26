using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;
using PlayFab.ClientModels;
using PlayFab;

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
    public Vector3 spawnPosition;   // �÷��̾� ���� ������
    private GameObject[] targetPlayer; //�÷��̾� ����Ʈ

    public bool isGameover { get; private set; } // ���� ���� ����
    public bool inputLock;  // �Է��� ���� �� �ִ� ����. true�� ���� �ɷ� �Է� �Ұ�

    public Transform shopPosition; // �� ���̺� ������Ʈ�Ǵ� ������ Ʈ������
    public int playerCount;
    [Header("Game Info")]

    public string playerNickName;
    public string playerLevel;

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
    public bool isCheck = false;        // ���� ���̺갡 ���� Ȯ��
    public bool isZedTimeCheck = false;
    public List<Transform> shops = new List<Transform>();
    public bool isShop = false;


    private bool GMMode = false;
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
        spawnPosition = new Vector3(0f, 1f, 0f);

        if (SceneManager.GetActiveScene().name == "Main")
        {
            Debug.Log("���ξ� ����");
            spawnPosition = new Vector3(135.0f, -6.0f, 200.0f);

        }

        // ��Ʈ��ũ ���� ��� Ŭ���̾�Ʈ�鿡�� ���� ����
        // ��, �ش� ���� ������Ʈ�� �ֵ�����, ���� �޼��带 ���� ������ Ŭ���̾�Ʈ���� ����
        //PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        if (GMMode)
        {
            Debug.Log("GM Mode");
            Transform bossRoomPos = FindAnyObjectByType<StartColiderScripts>().transform;
            newPlayer.transform.position = bossRoomPos.transform.position;
        }
        //newPlayer.transform.SetParent(GameObject.Find("Players").transform);

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
        GetPlayerData();

        // ToDO : �׽�Ʈ������ �Ѿ���� �����ǵ��� �����ϱ�

        StartCoroutine(StartWave());
    }

    // Ű���� �Է��� �����ϰ� ���� ������ ��
    private void Update()
    {
        SetPlayer();
        shopPosition = shops[wave - 1];
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            LeaveServer();
        }
    }

    public void zombieCount(int _currentZombieCount)
    {
        photonView.RPC("MasterCount", RpcTarget.MasterClient, _currentZombieCount);
    }

    [PunRPC]
    public void MasterCount(int _currentZombieCount)
    {
        photonView.RPC("SyncCount", RpcTarget.All, _currentZombieCount);
    }

    [PunRPC]
    public void SyncCount(int _currentZombieCount)
    {
        currentZombieCount = _currentZombieCount;
    }

    public void SetPlayer()
    {
        //if(playerCount == PhotonNetwork.CurrentRoom.PlayerCount)
        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject players in targetPlayer)
        {
            players.transform.SetParent(GameObject.Find("Players").transform);
        }
    }

    public void LeaveServer()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LoginScene");

    }

    // ���� ������ �ڵ� ����Ǵ� �޼���
    public override void OnLeftRoom()
    {
        // ���� ������ �κ� ������ ���ư�
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
    // ��ȯ �߰�
    public void GetPlayerData()
    {
        Debug.Log("�÷��̾� ������" + NetworkManager.instance.localPlayerName + "" + NetworkManager.instance.localPlayerLv);
        playerNickName = string.Format(NetworkManager.instance.localPlayerName);
        playerLevel = string.Format(NetworkManager.instance.localPlayerLv);

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

        isCheck = true;

        while (true)
        {
            if (currentZombieCount > 0) { break; }

            yield return null;
        }

        PlayerUIManager.instance.SetStartNotice("Start Wave");
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
        isShop = true;

        PlayerUIManager.instance.SetEndNotice("Wave Clear");
        PlayerUIManager.instance.SetNoticeLogo("Go to Shop");

        PlayerUIManager.instance.CountUI.SetActive(false);
        PlayerUIManager.instance.TimerUI.SetActive(true);

        PlayerUIManager.instance.zombieCountText.gameObject.SetActive(false);
        PlayerUIManager.instance.timerCountText.gameObject.SetActive(true);

        StartCoroutine(noticeController.CoroutineManager(true));

        int timeElapsed = 70;
    

        while (0 < timeElapsed)
        {
            timeElapsed -= 1;

            PlayerUIManager.instance.SetTimerCount(timeElapsed);

         
            yield return new WaitForSeconds(1);
        }

        isShop = false;
        SetWave(1);
        isCheck = true;
        StartCoroutine(StartWave());
    }
    // Junoh �߰�
}
