using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;
using PlayFab.ClientModels;
using PlayFab;
using Photon.Pun.Demo.Cockpit;
using Unity.VisualScripting;
using Photon.Pun.Demo.PunBasics;

public class GameManager : MonoBehaviourPunCallbacks
{

    public List<string> cashItem;
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private static GameManager m_instance; // 싱글톤이 할당될 static 변수

    public GameObject playerPrefab; // 생성할 플레이어 캐릭터 프리팹
    public Vector3 spawnPosition;   // 플레이어 스폰 포지션
    public GameObject[] targetPlayer; //플레이어 리스트
    public bool isGameover { get; private set; } // 게임 오버 상태
    public bool inputLock;  // 입력을 받을 수 있는 상태. true면 락이 걸려 입력 불가

    public Transform shopPosition; // 매 웨이브 업데이트되는 상점의 트랜스폼
    public int playerDieCount;
    [Header("Game Info")]

    public string playerNickName;
    public string playerLevel;
    public string playerClass;
    public int playerSkin;





    // 지환 추가

    [Header("Game Setting")]

    // Junoh 추가
    public NoticeController noticeController;
    public Volume volume;

    public int round = 1;               // 현재 라운드
    public int wave = 1;                // 현재 웨이브
    [SerializeField] public int MaxWave = 5;//마직막 웨이브
    public int player = 4;              // 플레이어 인원 수
    public int difficulty = 0;          // 난이도 0: 보통 1: 어려움 2: 지옥
    public int currentZombieCount = 0;  // 현재 좀비 수
    public bool isZedTime = false;      // 제드 타임
    public bool isSpawnZombie = false;  // 좀비가 소환 됬는지 확인
    public bool isCheck = false;        // 좀비 웨이브가 시작 확인
    public bool isZedTimeCheck = false;
    public List<Transform> shops = new List<Transform>();
    public bool isShop = false;
    private bool isRespawn = false;
    [SerializeField] private Transform zombieParent;

    public bool isWave = false;
    private bool isRest = false;



    private bool GMMode = false;
    // Junoh 추가
    
    [Header("Debug")]
    public bool palyerTestMode; // 지환 : 플레이어 테스트룸 

    private void Awake()
    {
        cashItem = new List<string>();
        //cashItem 에 저장

        //
        GetPlayerData();    // 로컬 플레이어 데이터 가져오기

        PhotonNetwork.AutomaticallySyncScene = true;


        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
        // 생성할 랜덤 위치 지정
        spawnPosition = new Vector3(0f, 1f, 0f);
        Quaternion newRotation = Quaternion.Euler(0, -90, 0);


        if (SceneManager.GetActiveScene().name == "Main")
        {
            Debug.Log("메인씬 입장");
            spawnPosition = new Vector3(135.0f, -6.0f, 200.0f);
        }

        // 네트워크 상의 모든 클라이언트들에서 생성 실행
        // 단, 해당 게임 오브젝트의 주도권은, 생성 메서드를 직접 실행한 클라이언트에게 있음
        //PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, newRotation);


        if (GMMode)
        {
            Debug.Log("GM Mode");
            Transform bossRoomPos = FindAnyObjectByType<StartColiderScripts>().transform;
            newPlayer.transform.position = bossRoomPos.transform.position;
        }
        //newPlayer.transform.SetParent(GameObject.Find("Players").transform);

    }

    // 키보드 입력을 감지하고 룸을 나가게 함
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && !palyerTestMode)    // 지환 : 플레이어 테스트 상태일경우 스킵
        {
            ZombieCountCheck();
            ZombieCount(currentZombieCount);

            if (GameObject.Find("LoadManager").GetComponent<LoadSceneAsync>().isCheck)
            {
                if (isWave)
                {
                    isWave = false;
                    WaveStart();
                }
                else if (isRest)
                {
                    isRest = false;
                    WaveChange();
                }
            }
        }

        SetPlayer();
        shopPosition = shops[wave - 1];
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            LeaveServer();
        }
        if (isRespawn)
        {
            OnRespawn();    // 상점이 열리면 리스폰 해주기
        }

        if (Input.GetKey(KeyCode.L) && isReset)
        {
            StartCoroutine(WaveReset());
        }
    }

    private bool isReset = false;

    private IEnumerator WaveReset()
    {
        isReset = true;

        WaveStart();

        yield return new WaitForSeconds(10);

        isReset = false;
    }

    #region WaveController
    public void WaveChange()
    {
        photonView.RPC("MasterChange", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void MasterChange()
    {
        photonView.RPC("SyncChange", RpcTarget.All);
    }

    [PunRPC]
    public void SyncChange()
    {
        StartCoroutine(ChangeWave());
    }

    public void WaveStart()
    {
        photonView.RPC("MasterStart", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void MasterStart()
    {
        photonView.RPC("SyncStart", RpcTarget.All);
    }

    [PunRPC]
    public void SyncStart()
    {
        StartCoroutine(StartWave());
    }
    #endregion

    #region ZombieCount
    public void ZombieCount(int _currentZombieCount)
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
    #endregion

    public void LeaveServer()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LoginScene");

        ////[Mijeong] 231001 : LocalPlayer Date 다시 불러옴
        //NetworkManager.net_instance.SetLocalPlayerData();
    }

    // 룸을 나갈때 자동 실행되는 메서드
    public override void OnLeftRoom()
    {
        // 룸을 나가면 로비 씬으로 돌아감
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("LoginScene");

        ////[Mijeong] 231001 : LocalPlayer Date 다시 불러옴
        //NetworkManager.net_instance.SetLocalPlayerData();
    }

    // 지환 추가
    public void GetPlayerData()
    {
        Debug.Log("플레이어 데이터" + NetworkManager.net_instance.localPlayerName + "" + NetworkManager.net_instance.localPlayerLv);
        playerNickName = string.Format(NetworkManager.net_instance.localPlayerName);
        playerLevel = string.Format(NetworkManager.net_instance.localPlayerLv);
        playerClass = NetworkManager.net_instance.localPlayerClass;
        playerSkin = NetworkManager.net_instance.localCashItem;

    }

    // Junoh 추가
    private void ZombieCountCheck()
    {
        int count = 0;

        for (int i = 0; i < zombieParent.childCount; i++)
        {
            for (int j = 0; j < zombieParent.GetChild(i).childCount; j++)
            {
                count += 1;
            }
        }

        currentZombieCount = count;
    }

    public void SetWave(int _num)
    {
        wave += _num;
    }

    private IEnumerator StartWave()
    {
        isCheck = true;

        PlayerUIManager.instance.CountUI.SetActive(true);
        PlayerUIManager.instance.TimerUI.SetActive(false);

        PlayerUIManager.instance.zombieCountText.gameObject.SetActive(true);
        PlayerUIManager.instance.timerCountText.gameObject.SetActive(false);

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

        PlayerUIManager.instance.SetZombieCount(currentZombieCount);

        if (PhotonNetwork.IsMasterClient)
        {
            isRest = true;
        }
    }

    private IEnumerator ChangeWave()
    {
        if (MaxWave == wave)
        {
            PlayerUIManager.instance.SetEndNotice("Clear");
            PlayerUIManager.instance.SetNoticeLogo("Congratulate");
        }
        else
        {
            PlayerUIManager.instance.SetEndNotice("Wave Clear");
            PlayerUIManager.instance.SetNoticeLogo("Go to Shop");
        }

        isShop = true;
        isRespawn = true;

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

        if (PhotonNetwork.IsMasterClient)
        {
            isWave = true;
        }
    }
    // Junoh 추가

    public void OnRespawn()
    {
        isRespawn = false;

        if (!isGameover)
        {
            // 죽은사람 카운트 초기화
            playerDieCount = 0;

            for (int i = 0; i <= targetPlayer.Length; i++)
            {
                PlayerHealth player = targetPlayer[i].GetComponent<PlayerHealth>();
                // 플레이어가 죽었을 경우 리스폰 시켜주기
                if (player.dead)
                {
                    player.Respawn();
                }
            }
        }

    }

    public void OnGameOver()
    {
        isGameover = true;
        PlayerUIManager.instance.gameOverUI.SetActive(true);
        PlayerUIManager.instance.leaveButton.SetActive(true);
        StartCoroutine(noticeController.GameOver(true));
        Debug.Log("게임오버");
    }
}
