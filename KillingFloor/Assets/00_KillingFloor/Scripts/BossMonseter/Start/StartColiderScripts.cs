using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartColiderScripts : MonoBehaviourPun
{
    private BoxCollider boxCollider; 
    private GameObject[] targetPlayer; //플레이어 리스트
    private List<GameObject> playersInTrigger; // 플레이어 콜라이더 접촉수 리스트저장용
    private GameObject Boss; // 보스오브젝트
    private List<GameObject> bossList;
    private GameObject entrance;
    private GameObject introCanvas;
    public GameObject bossPf;
    public GameObject bossPos;
    // Start is called before the first frame update

  
    void Start()
    {

        playersInTrigger = new List<GameObject>(); // 플레이어 콜라이더 접촉수 리스트저장용
        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
        entrance = GameObject.Find("Entrance");
        entrance.SetActive(false);
        boxCollider = GetComponent<BoxCollider>();
    
       
   
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
   
        if (playersInTrigger.Count >= targetPlayer.Length) //플레이어가 모두 입장시 보스 Active
        {
            
                PhotonNetwork.Instantiate(bossPf.name, bossPos.transform.position, bossPos.transform.rotation);
                entrance.SetActive(true);
               
                gameObject.SetActive(false);
            
        }
      
     
    }
   


    //접촉중인 플레이어
    private void OnTriggerStay(Collider other)
    {
      
        GameObject otherGameObject = other.gameObject;

        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
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
