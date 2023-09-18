using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 지환 추가 : 플레이어 조작 가능여부
    public bool inputEnable = true;
    // 지환 추가


    // Junoh 추가
    public NoticeController noticeController;

    public int round = 1;               // 현재 라운드
    public int wave = 1;                // 현재 웨이브
    public int player = 4;              // 플레이어 인원 수
    public int difficulty = 0;          // 난이도 0: 보통 1: 어려움 2: 지옥
    public int currentZombieCount = 0;  // 현재 좀비 수
    public bool isZedTime = false;      // 제드 타임
    public bool isSpawnZombie = false;  // 좀비가 소환 됬는지 확인
    public bool isCheck = true;         // 좀비 웨이브가 시작 확인
    // Junoh 추가

    public void Awake()
    {
        if (instance == null)
        { instance = this; }
        else
        { GlobalFunc.LogWarning("씬에 두 개 이상의 게임 매니저가 존재합니다."); }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartWave());
    }

    // Update is called once per frame
    void Update()
    {
        if (isZedTime)
        {
            Time.timeScale = 0.2f;
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
    // Junoh 추가
}
