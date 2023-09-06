using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NormalNavigation : MonoBehaviour
{
    private List<GameObject> targets = new List<GameObject>();  // Player가 저장되는 List

    public Transform Players;                                   // Player가 저장되는 부모 오브젝트

    private NavMeshAgent nav;                                   // 네비게이션

    private float minDistance;                                  // 가장 가까운 오브젝트의 거리
    private int minDistanceTarget;                              // 가장 가까운 오브젝트 List number

    private bool isCoroutine;                                   // 코루틴이 끝났는지 체크

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
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
        if (GetComponent<NormalZombie>().healthBody <= 0 || GetComponent<NormalZombie>().healthHead <= 0)
        {
            if (isCoroutine == false)
            {
                CheckIfInRadius();
                StartCoroutine(Target());
            }
        }
        else
        {
            GetComponent<NavMeshAgent>().enabled = false;
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
                else
                {
                    minDistance = minDistance < Mathf.Abs(distance) ? minDistance : Mathf.Abs(distance);
                    minDistanceTarget = minDistance < Mathf.Abs(distance) ? minDistanceTarget : i;
                }
            }

            yield return null;
        }

        if (GetComponent<NavMeshAgent>().enabled)
        {
            nav.SetDestination(targets[minDistanceTarget].transform.position);
        }   // if: 이동을 제외한 액션

        isCoroutine = false;
    }

    private void CheckIfInRadius()
    {
        float distance = Vector3.Distance(transform.position, targets[minDistanceTarget].transform.position);

        if (distance <= GetComponent<NavMeshAgent>().radius)
        {
            GetComponent<NavMeshAgent>().enabled = false;
        }
        else
        {
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }   // 공격 및 죽음을 확인하기 위해 NavMeshAgent를 끄는 로직
}
