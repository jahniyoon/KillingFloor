using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // ��ȯ �߰� : �÷��̾� ���� ���ɿ���
    public bool inputEnable = true;
    // ��ȯ �߰�


    // Junoh �߰�
    public NoticeController noticeController;

    public int round = 1;               // ���� ����
    public int wave = 1;                // ���� ���̺�
    public int player = 4;              // �÷��̾� �ο� ��
    public int difficulty = 0;          // ���̵� 0: ���� 1: ����� 2: ����
    public int currentZombieCount = 0;  // ���� ���� ��
    public bool isZedTime = false;      // ���� Ÿ��
    public bool isSpawnZombie = false;  // ���� ��ȯ ����� Ȯ��
    public bool isCheck = true;         // ���� ���̺갡 ���� Ȯ��
    // Junoh �߰�

    public void Awake()
    {
        if (instance == null)
        { instance = this; }
        else
        { GlobalFunc.LogWarning("���� �� �� �̻��� ���� �Ŵ����� �����մϴ�."); }
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
