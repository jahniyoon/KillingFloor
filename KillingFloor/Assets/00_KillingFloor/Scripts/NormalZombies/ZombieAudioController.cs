using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAudioController : MonoBehaviour
{
    [SerializeField]
    private AudioClip atkClip;
    [SerializeField]
    private AudioClip screamingClip;
    [SerializeField]
    private AudioClip deadClip;

    private Animator ani;
    private AudioSource audio;

    private bool isAtk = false;
    private bool isScreaming = false;
    private bool isdead = false;

    private void Awake()
    {
        ani = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAtk && ani.GetCurrentAnimatorStateInfo(0).IsName("Atk01") || ani.GetCurrentAnimatorStateInfo(0).IsName("Atk02")) { StartCoroutine(Atk()); }
        if (!isScreaming && ani.GetCurrentAnimatorStateInfo(0).IsName("Screaming")) { StartCoroutine(Screaming()); }
        if (!isdead && ani.GetCurrentAnimatorStateInfo(0).IsName("Dead")) { StartCoroutine(Dead()); }
    }

    private IEnumerator Atk()
    {
        isAtk = true;

        audio.Stop();
        audio.loop = false;
        audio.clip = atkClip;
        audio.Play();

        while (ani.GetCurrentAnimatorStateInfo(0).IsName("Atk01") || ani.GetCurrentAnimatorStateInfo(0).IsName("Atk02"))
        {
            yield return null;
        }

        audio.Stop();
        isAtk = false;
    }

    private IEnumerator Screaming()
    {
        isScreaming = true;

        audio.Stop();
        audio.loop = false;
        audio.clip = screamingClip;
        audio.Play();

        while (ani.GetCurrentAnimatorStateInfo(0).IsName("Screaming"))
        {
            yield return null;
        }

        audio.Stop();
        isScreaming = false;
    }

    private IEnumerator Dead()
    {
        isdead = true;

        audio.Stop();
        audio.loop = false;
        audio.clip = deadClip;
        audio.Play();

        while (ani.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            yield return null;
        }

        audio.Stop();
        isdead = false;
    }
}
