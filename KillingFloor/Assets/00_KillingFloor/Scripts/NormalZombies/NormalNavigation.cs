using Photon.Pun;
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

    private float length = 2.0f;                                // 좀비 근접 사정거리
    private float longLength = 8.0f;
    private float minDistance;                                  // 가장 가까운 오브젝트의 거리
    private int minDistanceTarget;                              // 가장 가까운 오브젝트 List number

    private bool isCoroutine;                                   // 코루틴이 끝났는지 체크
    public bool isContact;                                      // 물체와 부딪혔는지 체크
    public bool isLongContact;

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
        if (PhotonNetwork.IsMasterClient)
        {
            if (0 < normalZombie.health)
            {
                if (isCoroutine == false)
                {
                    nav.enabled = true;
                    StartCoroutine(Target());
                }
            }
            else
            {
                nav.enabled = false;
            }
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
        if (Vector3.Distance(transform.position, targets[minDistanceTarget].transform.position) <= longLength)
        {
            isLongContact = true;
        }
        else
        {
            isLongContact = false;
        }
        if (Vector3.Distance(transform.position, targets[minDistanceTarget].transform.position) <= length)
        {
            isContact = true;
        }
        else
        {
            isContact = false;

            if (targets[minDistanceTarget].transform.position == null)
            {
                nav.enabled = false;
            }
            else if (targets[minDistanceTarget].transform.position != null)
            {
                if (nav.enabled == true)
                {
                    nav.SetDestination(targets[minDistanceTarget].transform.position);
                }
            }
        }

        isCoroutine = false;
    }
}
