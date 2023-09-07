using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartColiderScripts : MonoBehaviour
{
    private BoxCollider boxCollider; 
    private GameObject[] targetPlayer; //플레이어 리스트
    private List<GameObject> playersInTrigger = new List<GameObject>(); // 플레이어 콜라이더 접촉수 리스트저장용
    private GameObject Boss; // 보스오브젝트
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
        if(playersInTrigger.Count >= targetPlayer.Length) //플레이어가 모두 입장시 보스 Active
        {
            if (!Boss.activeSelf)
            {
                Boss.SetActive(true);
                gameObject.SetActive(false);
            }
        }
      
     
    }

    //접촉중인 플레이어
    private void OnTriggerStay(Collider other)
    {
        GameObject otherGameObject = other.gameObject;


        // 트리거에 진입한 상대방이 플레이어인지 여부를 판단
        if (otherGameObject.CompareTag("Player"))
        {
            // 이미 리스트에 추가되지 않았으면 추가
            if (!playersInTrigger.Contains(otherGameObject))
            {
                playersInTrigger.Add(otherGameObject);
                // 플레이어 트리거 접촉 이벤트를 처리하거나 필요한 작업 수행
            }
        }
    }


    // 접촉을 끊은 플레이어
    private void OnTriggerExit(Collider other)
    {
        GameObject otherGameObject = other.gameObject;

        // 트리거에서 나간 상대방이 플레이어인지 여부를 판단
        if (otherGameObject.CompareTag("Player"))
        {
            // 리스트에서 제거
            if (playersInTrigger.Contains(otherGameObject))
            {
                playersInTrigger.Remove(otherGameObject);
                // 플레이어 트리거 접촉 해제 이벤트를 처리하거나 필요한 작업 수행
            }
        }
    }


}
