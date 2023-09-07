using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartColiderScripts : MonoBehaviour
{
    private BoxCollider boxCollider; 
    private GameObject[] targetPlayer; //�÷��̾� ����Ʈ
    private List<GameObject> playersInTrigger = new List<GameObject>(); // �÷��̾� �ݶ��̴� ���˼� ����Ʈ�����
    private GameObject Boss; // ����������Ʈ
    // Start is called before the first frame update

    private void Awake()
    {
        Boss = FindAnyObjectByType<BossController>().gameObject;
        Boss.SetActive(false);
    }
    void Start()
    {
 
        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
        boxCollider = GetComponent<BoxCollider>();  

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(playersInTrigger);
        if(playersInTrigger.Count >= targetPlayer.Length) //�÷��̾ ��� ����� ���� Active
        {
            if (!Boss.activeSelf)
            {
                Boss.SetActive(true);
                gameObject.SetActive(false);
            }
        }
      
     
    }

    //�������� �÷��̾�
    private void OnTriggerStay(Collider other)
    {
        GameObject otherGameObject = other.gameObject;


        // Ʈ���ſ� ������ ������ �÷��̾����� ���θ� �Ǵ�
        if (otherGameObject.CompareTag("Player"))
        {
            // �̹� ����Ʈ�� �߰����� �ʾ����� �߰�
            if (!playersInTrigger.Contains(otherGameObject))
            {
                playersInTrigger.Add(otherGameObject);
                // �÷��̾� Ʈ���� ���� �̺�Ʈ�� ó���ϰų� �ʿ��� �۾� ����
            }
        }
    }


    // ������ ���� �÷��̾�
    private void OnTriggerExit(Collider other)
    {
        GameObject otherGameObject = other.gameObject;

        // Ʈ���ſ��� ���� ������ �÷��̾����� ���θ� �Ǵ�
        if (otherGameObject.CompareTag("Player"))
        {
            // ����Ʈ���� ����
            if (playersInTrigger.Contains(otherGameObject))
            {
                playersInTrigger.Remove(otherGameObject);
                // �÷��̾� Ʈ���� ���� ���� �̺�Ʈ�� ó���ϰų� �ʿ��� �۾� ����
            }
        }
    }


}
