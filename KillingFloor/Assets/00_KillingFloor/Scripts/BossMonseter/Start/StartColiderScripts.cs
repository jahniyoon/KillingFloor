using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartColiderScripts : MonoBehaviour
{
    private BoxCollider boxCollider; 
    private GameObject[] targetPlayer; //�÷��̾� ����Ʈ
    private List<GameObject> playersInTrigger; // �÷��̾� �ݶ��̴� ���˼� ����Ʈ�����
    private GameObject Boss; // ����������Ʈ
    private GameObject entrance;
    private GameObject introCanvas;
    // Start is called before the first frame update

    private void Awake()
    {
        Boss = FindAnyObjectByType<BossController>().gameObject;
        Boss.SetActive(false);
       
    }
    void Start()
    {
        playersInTrigger = new List<GameObject>(); // �÷��̾� �ݶ��̴� ���˼� ����Ʈ�����
        entrance = GameObject.Find("Entrance");
        entrance.SetActive(false);


        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
        boxCollider = GetComponent<BoxCollider>();
        Debug.Log(targetPlayer.Length);

    }

    // Update is called once per frame
    void Update()
    {
      
        if (playersInTrigger.Count >= targetPlayer.Length) //�÷��̾ ��� ����� ���� Active
        {
            if (!Boss.activeSelf)
            {

                entrance.SetActive(true);
               
                Boss.SetActive(true);
                gameObject.SetActive(false);
            }
        }
      
     
    }

    //�������� �÷��̾�
    private void OnTriggerStay(Collider other)
    {
        GameObject otherGameObject = other.gameObject;

        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
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
