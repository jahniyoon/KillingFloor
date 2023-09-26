using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextAlpha : MonoBehaviour
{
    private Text text;

    private Color colorUp = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    private Color colorDown = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    float timeElapsed = 0.0f;

    void Start()
    {
        text = GetComponent<Text>();

        StartCoroutine(AlphaUp());
    }

    private IEnumerator AlphaUp()
    {
        {
            timeElapsed = 0.0f;

            while (timeElapsed < 2.0f)
            {
                timeElapsed += Time.deltaTime;

                float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / 2.0f), 2);

                text.color = Color.Lerp(colorDown, colorUp, time);

                yield return null;

                if (!gameObject.activeSelf)
                {
                    yield break;
                }
            }

            StartCoroutine(AlphaDown());
        }
    }

    private IEnumerator AlphaDown()
    {
        timeElapsed = 0.0f;

        while (timeElapsed < 2.0f)
        {
            timeElapsed += Time.deltaTime;

            float time = Mathf.Clamp01(timeElapsed / 2.0f);

            text.color = Color.Lerp(colorUp, colorDown, time * time);

            yield return null;

            if (!gameObject.activeSelf)
            {
                yield break;
            }
        }

        StartCoroutine(AlphaUp());
    }
}
