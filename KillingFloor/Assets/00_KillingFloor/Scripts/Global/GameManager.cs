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
    private GameObject[] targetPlayer; //플레이어 리스트

    public bool isGameover { get; private set; } // 게임 오버 상태
    public bool inputLock;  // 입력을 받을 수 있는 상태. true면 락이 걸려 입력 불가

    public Transform shopPosition; // 매 웨이브 업데이트되는 상점의 트랜스폼

    // 지환 추가

    [Header("Game Setting")]

    // Junoh 추가
    public NoticeController noticeController;
    public Volume volume;

    public int round = 1;               // 현재 라운드
    public int wave = 1;                // 현재 웨이브
    public int player = 4;              // 플레이어 인원 수
    public int difficulty = 0;          // 난이도 0: 보통 1: 어려움 2: 지옥
    public int currentZombieCount = 0;  // 현재 좀비 수
    public bool isZedTime = false;      // 제드 타임
    public bool isSpawnZombie = false;  // 좀비가 소환 됬는지 확인
    public bool isCheck = false;        // 좀비 웨이브가 시작 확인
    public bool isZedTimeCheck = false;
    public List<Transform> shops = new List<Transform>();
    public bool isShop = false;
    // Junoh 추가

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
        // 생성할 랜덤 위치 지정
        spawnPosition = new Vector3(0f, 1f, 0f);

        if (SceneManager.GetActiveScene().name == "Main")
        {
            Debug.Log("메인씬 입장");
            spawnPosition = new(135.0f, -6.0f, 200.0f);

        }

        // 네트워크 상의 모든 클라이언트들에서 생성 실행
        // 단, 해당 게임 오브젝트의 주도권은, 생성 메서드를 직접 실행한 클라이언트에게 있음
        //PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);


    }

    //private void Awake()
    //{
    //    // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
    //    if (instance != this)
    //    {
    //        // 자신을 파괴
    //        Destroy(gameObject);
    //    }
    //    if (instance == null)
    //    { instance = this; }
    //    else
    //    { GlobalFunc.LogWarning("씬에 두 개 이상의 게임 매니저가 존재합니다."); }
    //}

    // Start is called before the first frame update
    void Start()
    {
        // ToDO : 테스트씬으로 넘어오면 생성되도록 수정하기

        StartCoroutine(StartWave());
    }

    // 키보드 입력을 감지하고 룸을 나가게 함
    private void Update()
    {
        SetPlayer();
        shopPosition = shops[wave - 1];
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.Log("서버 나가기 시도");

            LeaveServer();
        }
        if (isZedTime) { StartCoroutine(ZedTime()); Debug.Log("몇번 호출 하는가?"); }
    }

    public void SetPlayer()
    {
        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject players in targetPlayer)
        {
            players.transform.SetParent(GameObject.Find("Players").transform);

        }
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
        timeElapsed = 0.0f;

        while (timeElapsed < 6.0 * 0.2f)
        {
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        timeElapsed = 0.0f;

        while (timeElapsed < 0.5f)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / 0.5f), 2);

            Time.timeScale = Mathf.Lerp(0.2f, 1.0f, time);
            volume.weight = Mathf.Lerp(1.0f, 0.0f, time);

            yield return null;
        }

        isZedTimeCheck = false;
    }
    public void LeaveServer()
    {
        Debug.Log("Leave Server");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LoginScene");

    }

    // 룸을 나갈때 자동 실행되는 메서드
    public override void OnLeftRoom()
    {
        // 룸을 나가면 로비 씬으로 돌아감
        SceneManager.LoadScene("LoginScene");
    }


    // 주기적으로 자동 실행되는, 동기화 메서드
    // ToDo : 좀비 웨이브, 좀비 카운트 등 업데이트 필요하면 부탁해요
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {
            // 네트워크를 통해 round 값을 보내기
            //stream.SendNext(currentZombieCount);
        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨         

            // 네트워크를 통해 score 값 받기
            //currentZombieCount = (int)stream.ReceiveNext();
            // 동기화하여 받은 점수를 UI로 표시
        }

    }





    // Junoh 추가
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
        isShop = true;

        PlayerUIManager.instance.SetNotice("Wave Clear");
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
    // Junoh 추가
}
