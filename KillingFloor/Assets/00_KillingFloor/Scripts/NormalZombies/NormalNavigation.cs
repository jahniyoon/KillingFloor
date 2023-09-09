using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NormalNavigation : MonoBehaviour
{
    private List<GameObject> targets = new List<GameObject>();  // Player�� ����Ǵ� List

    private NormalZombie normalZombie;
    private Transform Players;                                   // Player�� ����Ǵ� �θ� ������Ʈ

    private NavMeshAgent nav;                                   // �׺���̼�

    private float minDistance;                                  // ���� ����� ������Ʈ�� �Ÿ�
    private int minDistanceTarget;                              // ���� ����� ������Ʈ List number

    private bool isCoroutine;                                   // �ڷ�ƾ�� �������� üũ
    public bool isContact;                                      // ��ü�� �ε������� üũ

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

        // ���� ������ Players�� �ڽ� ������Ʈ ������ŭ List�� �߰�
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
        minDistance = 0.0f;     // minDistance �ʱ�ȭ
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].activeSelf == true)      // if: �ɸ��Ͱ� �׾����� Ȯ��
            {
                float distance = Vector3.Distance(GetComponent<Transform>().position, targets[i].transform.position);

                if (minDistance == 0.0f)
                {
                    minDistance = distance;
                }   // if: minDistance �� 0.0f �� ���� ����ó��
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
    }   // ���� �� ������ Ȯ���ϱ� ���� NavMeshAgent�� ���� ����
}
