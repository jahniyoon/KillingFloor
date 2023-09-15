using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager instance;

    // ��ȯ �߰� : �÷��̾� ���� ���ɿ���
    public bool isGameover { get; private set; } // ���� ���� ����
    public bool inputEnable = true;
    public GameObject playerPrefab; // ������ �÷��̾� ĳ���� ������

    // ��ȯ �߰�

    [Header("Game Setting")]

    // Junoh �߰�
    public int round = 1;       // ���� ����
    public int player = 4;      // �÷��̾� �ο� ��
    public int difficulty = 0;  // ���̵� 0: ���� 1: ����� 2: ����
    public int currentZombieCount = 0; // ���� ���� ��
    // Junoh �߰�


    public void Awake()
    {
        if(instance == null)
        { instance = this; }
        else
        { GlobalFunc.LogWarning("���� �� �� �̻��� ���� �Ŵ����� �����մϴ�."); }
    }

    // Start is called before the first frame update
    void Start()
    {
        // ������ ���� ��ġ ����
        Vector3 spawnPosition = new Vector3 (8f,0f,8f);

        // ��Ʈ��ũ ���� ��� Ŭ���̾�Ʈ�鿡�� ���� ����
        // ��, �ش� ���� ������Ʈ�� �ֵ�����, ���� �޼��带 ���� ������ Ŭ���̾�Ʈ���� ����
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
    }

    // Ű���� �Է��� �����ϰ� ���� ������ ��
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    // ���� ������ �ڵ� ����Ǵ� �޼���
    public override void OnLeftRoom()
    {
        // ���� ������ �κ� ������ ���ư�
        SceneManager.LoadScene("Lobby");
    }


    // �ֱ������� �ڵ� ����Ǵ�, ����ȭ �޼���
    // ToDo : ���� ���̺�, ���� ī��Ʈ �� ������Ʈ �ʿ��ϸ� ��Ź�ؿ�
    // �ϰ� ���˾�! - To : �ؿ�
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
    // Junoh �߰�
}
