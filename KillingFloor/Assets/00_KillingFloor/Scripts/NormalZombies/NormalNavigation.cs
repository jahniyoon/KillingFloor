using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NormalNavigation : MonoBehaviour
{
    private List<GameObject> targets = new List<GameObject>();  // Player가 저장되는 List

    private NormalZombie normalZombie;
    private Transform Players;                                   // Player가 저장되는 부모 오브젝트

    private NavMeshAgent nav;                                   // 네비게이션

    private float minDistance;                                  // 가장 가까운 오브젝트의 거리
    private int minDistanceTarget;                              // 가장 가까운 오브젝트 List number

    private bool isCoroutine;                                   // 코루틴이 끝났는지 체크
    public bool isContact;                                      // 물체와 부딪혔는지 체크

    private void Awake()
    {
        normalZombie = GetComponent<NormalZombie>();
        nav = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        Players = GameObject.Find("Players").transform;
    }

    private void Start()
    {
        Players = GameObject.Find("Players").transform;

        // 좀비 생성시 Players의 자식 오브젝트 갯수만큼 List에 추가
        if (Players.childCount != 0)
        {
            for (int i = 0; i < Players.childCount; i++)
            {
                targets.Add(Players.GetChild(i).gameObject);
            }
        }
    }

    private void Update()
    {
        if (0.0f <= normalZombie.health)
        {
            if (isCoroutine == false)
            {
                StartCoroutine(Target());
            }
        }
        else
        {
            nav.enabled = false;
        }
    }

    private IEnumerator Target()
    {
        isCoroutine = true;
        minDistance = 0.0f;     // minDistance 초기화
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].activeSelf == true)      // if: 케릭터가 죽었는지 확인
            {
                float distance = Vector3.Distance(GetComponent<Transform>().position, targets[i].transform.position);

                if (minDistance == 0.0f)
                {
                    minDistance = distance;
                }   // if: minDistance 가 0.0f 일 때를 예외처리
                if (minDistance != 0.0f)
                {
                    minDistance = minDistance < Mathf.Abs(distance) ? minDistance : Mathf.Abs(distance);
                    minDistanceTarget = minDistance < Mathf.Abs(distance) ? minDistanceTarget : i;
                }
            }

            yield return null;
        }

        nav.SetDestination(targets[minDistanceTarget].transform.position);
        
        CheckIfInRadius();

        isCoroutine = false;
    }

    private void CheckIfInRadius()
    {
        float distance = Vector3.Distance(transform.position, targets[minDistanceTarget].transform.position);

        if (distance <= nav.radius)
        {
            nav.speed = 0;

            isContact = true;
        }
        else
        {
            nav.speed = 0.1f;

            isContact = false;
        }
    }   // 공격 및 죽음을 확인하기 위해 NavMeshAgent를 끄는 로직
}
