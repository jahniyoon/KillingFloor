using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackNoise : MonoBehaviour
{
    private float timeElapsed = 0.0f;
    private List<GameObject> targets = new List<GameObject>();
    private Transform players;

    private bool isCoroutine = false;

    private void Awake()
    {
        players = GameObject.Find("Players").transform;
    }

    private void OnEnable()
    {
        if (isCoroutine == false)
        {
            StartCoroutine(Attack());
        }
    }
    private void Start()
    {
        if (isCoroutine == false)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isCoroutine = true;

        if (players.childCount != targets.Count)
        {
            for (int i = 0; i < players.childCount; i++)
            {
                targets.Add(players.GetChild(i).gameObject);
            }
        }

        timeElapsed = 0.0f;

        while (timeElapsed < 1.0f)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (Vector3.Distance(gameObject.transform.position, targets[i].transform.position) <= 8.0f)
                {
                    targets[i].GetComponent<PlayerHealth>().OnDamage(1.0f, Vector3.zero, Vector3.zero);
                }
            }

            yield return new WaitForSeconds(0.1f);

            timeElapsed += 0.1f;
        }

        isCoroutine = false;
    }
}
