using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpit : MonoBehaviour
{
    private CapsuleCollider collider;

    private float timeElapsed = 0.0f;
    private float time = 0.0f;

    private bool isCoroutine = false;

    private void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
        collider.enabled = false;
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

        timeElapsed = 0.0f;

        collider.enabled = true;

        while (timeElapsed < 0.8f)
        {
            timeElapsed += Time.deltaTime;

            time = Mathf.Clamp01(timeElapsed / 0.8f);

            collider.center = new Vector3(0.0f, 0.0f, Mathf.Lerp(0.0f, 15.0f, time));
            collider.height = Mathf.Lerp(0.0f, 30.0f, time);

            yield return null;
        }

        timeElapsed = 0.0f;

        while (timeElapsed < 0.6f)
        {
            timeElapsed += Time.deltaTime;

            time = Mathf.Clamp01(timeElapsed / 0.6f);

            collider.center = new Vector3(0.0f, 0.0f, Mathf.Lerp(15.0f, 30.0f, time));

            yield return null;
        }

        timeElapsed = 0.0f;

        while (timeElapsed < 0.8f)
        {
            timeElapsed += Time.deltaTime;

            time = Mathf.Clamp01(timeElapsed / 0.8f);

            collider.center = new Vector3(0.0f, 0.0f, Mathf.Lerp(30.0f, 45.0f, time));
            collider.height = Mathf.Lerp(30.0f, 5.0f, time);

            yield return null;
        }

        collider.enabled = false;

        isCoroutine = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().startingHealth -= 1.0f;
        }
    }
}