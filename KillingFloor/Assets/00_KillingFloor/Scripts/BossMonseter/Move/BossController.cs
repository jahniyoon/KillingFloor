using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private GameObject[] targetPlayer;
    private int randPlayerNum;
    private Animator animator;
    private int randomFattern;
    private float currentTime = 0f;
    private float setTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        randomFattern = Random.Range(0,6);
        animator = GetComponent<Animator>();    
        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
        randPlayerNum = Random.Range(0, targetPlayer.Length);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= setTime)
        {
            LookRotate();
            BossMove();
        }

    }
    private void LookRotate()
    {
        if (targetPlayer[randPlayerNum] == null)
        {
            randPlayerNum = Random.Range(0, targetPlayer.Length);
        }

        Vector3 lookDirection = targetPlayer[randPlayerNum].transform.position - transform.position;

        lookDirection = new Vector3(lookDirection.x, 0f, lookDirection.z);
        // 대상을 바라보는 회전값(Quaternion) 계산
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        // 현재 오브젝트의 회전을 대상을 바라보는 회전으로 설정
        transform.rotation = lookRotation;
    }

    private void BossMove()
    {

        Vector3 targetVector = transform.position - targetPlayer[randPlayerNum].transform.position;
        transform.Translate(Vector3.forward * 5 * Time.deltaTime); ;
    }
}
    