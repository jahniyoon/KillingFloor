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
    public bool zedTime = false;        // ���� Ÿ��
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
    // Junoh �߰�
}
