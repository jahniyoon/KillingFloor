using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeController : MonoBehaviourPun
{
    private Coroutine zedCoroutine;
    private Volume volume;

    public bool isZedTime = false;
    private bool isZedTimeCheck = false;

    private void Awake()
    {
        volume = GetComponent<Volume>();    
    }

    private void Update()
    {
        if (isZedTime && photonView.IsMine) { ZedTimeStart(); }
    }

    public void ZedTimeStart()
    {
        photonView.RPC("MasterZedTime", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void MasterZedTime()
    {
        photonView.RPC("SyncZedTime", RpcTarget.All);
    }

    [PunRPC]
    public void SyncZedTime()
    {
        if (zedCoroutine != null) { StopCoroutine(zedCoroutine); }
        zedCoroutine = StartCoroutine(ZedTime());
    }

    public IEnumerator ZedTime()
    {
        isZedTime = false;

        float timeElapsed = 0.0f;

        if (isZedTimeCheck == false)
        {
            while (timeElapsed < 0.5f)
            {
                timeElapsed += Time.deltaTime;

                float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / 0.5f), 2);

                Time.timeScale = Mathf.Lerp(1.0f, 0.2f, time);
                volume.weight = Mathf.Lerp(0.0f, 1.0f, time);

                yield return null;
            }

            isZedTimeCheck = true;
        }
        timeElapsed = 0.0f;

        while (timeElapsed < 6.0 * 0.2f)
        {
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        timeElapsed = 0.0f;

        while (timeElapsed < 0.5f)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / 0.5f), 2);

            Time.timeScale = Mathf.Lerp(0.2f, 1.0f, time);
            volume.weight = Mathf.Lerp(1.0f, 0.0f, time);

            yield return null;
        }

        isZedTimeCheck = false;
    }
}
