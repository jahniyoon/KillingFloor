using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Junoh �߰�
    public int round = 1;       // ���� ����
    public int player = 4;      // �÷��̾� �ο� ��
    public int difficulty = 0;  // ���̵� 0: ���� 1: ����� 2: ����
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
