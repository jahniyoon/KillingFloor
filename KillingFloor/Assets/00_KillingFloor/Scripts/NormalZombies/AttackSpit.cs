using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpit : MonoBehaviour
{
    private Coroutine coroutine;
    private CapsuleCollider capsuleCollider;

    private float timeElapsed = 0.0f;
    private float time = 0.0f;

    private bool isCoroutine = false;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = false;
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

        capsuleCollider.enabled = true;

        while (timeElapsed < 0.8f)
        {
            timeElapsed += Time.deltaTime;

            time = Mathf.Clamp01(timeElapsed / 0.8f);

            capsuleCollider.center = new Vector3(0.0f, 0.0f, Mathf.Lerp(0.0f, 15.0f, time));
            capsuleCollider.height = Mathf.Lerp(0.0f, 30.0f, time);

            yield return null;
        }

        timeElapsed = 0.0f;

        while (timeElapsed < 0.6f)
        {
            timeElapsed += Time.deltaTime;

            time = Mathf.Clamp01(timeElapsed / 0.6f);

            capsuleCollider.center = new Vector3(0.0f, 0.0f, Mathf.Lerp(15.0f, 30.0f, time));

            yield return null;
        }

        timeElapsed = 0.0f;

        while (timeElapsed < 0.8f)
        {
            timeElapsed += Time.deltaTime;

            time = Mathf.Clamp01(timeElapsed / 0.8f);

            capsuleCollider.center = new Vector3(0.0f, 0.0f, Mathf.Lerp(30.0f, 45.0f, time));
            capsuleCollider.height = Mathf.Lerp(30.0f, 5.0f, time);

            yield return null;
        }

        capsuleCollider.enabled = false;

        isCoroutine = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        { return; }
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().OnPoison();

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(PoisonAttack(other));
        }
    }

    private IEnumerator PoisonAttack(Collider _other)
    {
        while (0 < _other.gameObject.GetComponent<PlayerInfoUI>().poisonScreenValue)
        {
            _other.gameObject.GetComponent<PlayerHealth>().OnDamage(1.0f, Vector3.zero, Vector3.zero);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
