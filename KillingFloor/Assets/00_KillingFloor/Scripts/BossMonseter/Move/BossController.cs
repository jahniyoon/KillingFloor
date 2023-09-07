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
        // ����� �ٶ󺸴� ȸ����(Quaternion) ���
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        // ���� ������Ʈ�� ȸ���� ����� �ٶ󺸴� ȸ������ ����
        transform.rotation = lookRotation;
    }

    private void BossMove()
    {

        Vector3 targetVector = transform.position - targetPlayer[randPlayerNum].transform.position;
        transform.Translate(Vector3.forward * 5 * Time.deltaTime); ;
    }
}
    