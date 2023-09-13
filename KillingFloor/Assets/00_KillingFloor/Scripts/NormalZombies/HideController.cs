using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HideController : MonoBehaviour
{
    public NormalNavigation nav;

    private float thisValue;
    private float timeElapsed = 0.0f;

    private bool isCoroutine = false;

    private void Awake()
    {
        nav = transform.parent.GetComponent<NormalNavigation>();
    }

    private void OnEnable()
    {
        thisValue = 0.0f;
        ChildSplit(gameObject, 0.0f, 1.0f);
    }

    private void Start()
    {
        thisValue = 0.0f;
        ChildSplit(gameObject, 0.0f, 1.0f);
    }

    private void Update()
    {
        if (isCoroutine == false)
        {
            if (nav.isLongContact == true)
            {
                if (thisValue != 1.0f)
                {
                    StartCoroutine(SplitChange(1.0f, 1.0f));
                }
            }
            else if (nav.isLongContact == false)
            {
                if (thisValue != 0.0f)
                {
                    StartCoroutine(SplitChange(0.0f, 1.0f));
                }
            }
        }
    }

    private IEnumerator SplitChange(float target, float duration)
    {
        isCoroutine = true;

        timeElapsed = 0.0f;

        thisValue = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.GetFloat("_SplitValue");

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            float time = Mathf.Clamp01(timeElapsed / duration);

            ChildSplit(gameObject, target, time);

            yield return null;
        }

        isCoroutine = false;
    }

    private void ChildSplit(GameObject gameObject, float target, float time)
    {
        if (gameObject.transform.childCount > 0)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                ChildSplit(gameObject.transform.GetChild(i).gameObject, target, time);
            }
        }
        else
        {
            if (gameObject.GetComponent<SkinnedMeshRenderer>() != null)
            {
                gameObject.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_SplitValue", Mathf.Lerp(thisValue, target, time));
            }
            else { /*No Event*/ }
        }
    }
}
