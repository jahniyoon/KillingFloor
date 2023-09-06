using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NormalNavigation : MonoBehaviour
{
    private List<GameObject> targets = new List<GameObject>();  // Player�� ����Ǵ� List

    public Transform Players;                                   // Player�� ����Ǵ� �θ� ������Ʈ

    private NavMeshAgent nav;                                   // �׺���̼�

    private float minDistance;                                  // ���� ����� ������Ʈ�� �Ÿ�
    private int minDistanceTarget;                              // ���� ����� ������Ʈ List number

    private bool isCoroutine;                                   // �ڷ�ƾ�� �������� üũ

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
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
        }   // if: �̵��� ������ �׼�

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
    }   // ���� �� ������ Ȯ���ϱ� ���� NavMeshAgent�� ���� ����
}
