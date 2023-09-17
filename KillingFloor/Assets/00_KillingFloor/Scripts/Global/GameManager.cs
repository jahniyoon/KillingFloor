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
    public bool zedTime = false;        // 제드 타임
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

    private IEnumerator StartWave()
    {
        PlayerUIManager.instance.SetNotice("Start Wave");

        StartCoroutine(noticeController.CoroutineManager(false));

        while (currentZombieCount <= 0)
        {
            yield return null;
        }

        StartCoroutine(ChangeWave());
    }

    private IEnumerator ChangeWave()
    {
        PlayerUIManager.instance.SetNotice("Wave Clear");
        PlayerUIManager.instance.SetNoticeLogo("Go to Shop");

        StartCoroutine(noticeController.CoroutineManager(true));

        int timeElapsed = 70;

        while (0 < timeElapsed)
        {
            timeElapsed -= 1;

            PlayerUIManager.instance.SetTimerCount(timeElapsed);

            yield return new WaitForSeconds(1);
        }

        StartCoroutine(StartWave());
    }
    // Junoh 추가
}
