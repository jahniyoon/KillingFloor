using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavShop : MonoBehaviour
{
    private int wave = 0;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
     
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.isShop == true)
        {
            wave = GameManager.instance.wave;
            string shopName = "Shop_0" + wave;
            GameObject shopObj = GameObject.Find(shopName);
            float targetDistance =  Vector3.Distance(transform.position,shopObj.transform.position);
            agent.enabled = true;
            if (shopObj != null)
            {
                agent.SetDestination(shopObj.transform.position);
                //Debug.Log("네비 상대 위치 " + targetDistance);
                if (targetDistance <= 1f)
                {
                    gameObject.SetActive(false);
                }
            }
        }
            
       if (GameManager.instance.isShop == false)
        {
            gameObject.SetActive(false);
        }
       
    }
}
