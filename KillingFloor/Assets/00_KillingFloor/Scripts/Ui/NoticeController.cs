using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeController : MonoBehaviour
{
    public RectTransform warningMain;

    public RectTransform warningSubLeft;
    public RectTransform warningSubMiddle;
    public RectTransform warningSubRight;

    public RectTransform noticeLeft;
    public RectTransform noticeMiddle;
    public RectTransform noticeRight;

    public CanvasGroup warningEndText;
    public CanvasGroup warningStartText;
    public CanvasGroup noticeTextText;
    public CanvasGroup noticeTextCount;
    public CanvasGroup GameOverWarning;
    public CanvasGroup GameOverNotice;

    private Vector2 initalLeft;
    private Vector2 initalRight;
    private Vector2 initalMiddle;

    private float timeElapsed = 0.0f;

    public bool isText = false;

    private void Awake()
    {
        warningMain.localScale = new Vector2(0.0f, 0.0f);
        warningSubLeft.localScale = new Vector2(0.0f, 0.0f);
        warningSubMiddle.localScale = new Vector2(0.0f, 0.0f);
        warningSubRight.localScale = new Vector2(0.0f, 0.0f);
        noticeLeft.localScale = new Vector2(0.0f, 0.0f);
        noticeMiddle.localScale = new Vector2(0.0f, 0.0f);
        noticeRight.localScale = new Vector2(0.0f, 0.0f);
        warningEndText.alpha = 0.0f;
        warningStartText.alpha = 0.0f;
        noticeTextText.alpha = 0.0f;
        noticeTextCount.alpha = 0.0f;
        GameOverWarning.alpha = 0.0f;
        GameOverNotice.alpha = 0.0f;
    }

    public IEnumerator CoroutineManager(bool _isText)
    {
        StartCoroutine(WarningMainScale(0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningMainScale(0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningSubScale(0.1f, false));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningSubMove(new Vector2(-300.0f, 0.0f), new Vector2(300.0f, 0.0f), new Vector2(60.0f, 0.0f), 0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningSubText(0.1f, false, _isText));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(NoticeScale(0.1f, false, _isText));
        yield return new WaitForSeconds(0.1f);
        if (_isText) { StartCoroutine(NoticeMove(new Vector2(-300.0f, 0.0f), new Vector2(300.0f, 0.0f), new Vector2(60.0f, 0.0f), 0.1f)); }
        else { StartCoroutine(NoticeMove(new Vector2(-100.0f, 0.0f), new Vector2(100.0f, 0.0f), new Vector2(20.0f, 0.0f), 0.1f)); }
        StartCoroutine(NoticeText(0.1f, false, _isText));
        yield return new WaitForSeconds(5.0f);
        StartCoroutine(NoticeText(0.1f, true, _isText));
        if (_isText) { StartCoroutine(NoticeMove(new Vector2(300.0f, 0.0f), new Vector2(-300.0f, 0.0f), new Vector2(-60.0f, 0.0f), 0.1f)); }
        else { StartCoroutine(NoticeMove(new Vector2(100.0f, 0.0f), new Vector2(-100.0f, 0.0f), new Vector2(-20.0f, 0.0f), 0.1f)); }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(NoticeScale(0.1f, true, _isText));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningSubText(0.1f, true, _isText));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningSubMove(new Vector2(300.0f, 0.0f), new Vector2(-300.0f, 0.0f), new Vector2(-60.0f, 0.0f), 0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningSubScale(0.1f, true));
    }

    public IEnumerator GameOver(bool _isText)
    {
        StartCoroutine(WarningMainScale(0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningMainScale(0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningSubScale(0.1f, false));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(WarningSubMove(new Vector2(-300.0f, 0.0f), new Vector2(300.0f, 0.0f), new Vector2(60.0f, 0.0f), 0.1f));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(GameOverText(0.1f, GameOverWarning));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(NoticeScale(0.1f, false, _isText));
        yield return new WaitForSeconds(0.1f);
        if (_isText) { StartCoroutine(NoticeMove(new Vector2(-300.0f, 0.0f), new Vector2(300.0f, 0.0f), new Vector2(60.0f, 0.0f), 0.1f)); }
        else { StartCoroutine(NoticeMove(new Vector2(-100.0f, 0.0f), new Vector2(100.0f, 0.0f), new Vector2(20.0f, 0.0f), 0.1f)); }
        StartCoroutine(GameOverText(0.1f, GameOverNotice));
    }

    private IEnumerator WarningMainScale(float _duration)
    {
        timeElapsed = 0.0f;

        warningMain.localScale = new Vector2(1.2f, 1.2f);

        while (timeElapsed < _duration * 0.3f)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / (_duration * 0.3f)), 2);

            warningMain.localScale = Vector2.Lerp(new Vector2(0.0f, 0.0f), new Vector2(1.2f, 1.2f), time);

            yield return null;
        }

        yield return new WaitForSeconds(_duration * 0.2f);

        timeElapsed = 0.0f;

        while (timeElapsed < _duration * 0.5f)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / (_duration * 0.5f)), 2);

            warningMain.localScale = Vector2.Lerp(new Vector2(1.2f, 1.2f), new Vector2(0.0f, 0.0f), time);

            yield return null;
        }

        warningMain.localScale = new Vector2(0.0f, 0.0f);
    }

    private IEnumerator WarningSubScale(float _duration, bool _isEnd)
    {
        timeElapsed = 0.0f;

        initalLeft = new Vector2(0.7f, 0.7f);
        initalMiddle = new Vector2(0.7f, 0.7f);
        initalRight = new Vector2(0.7f, 0.7f);

        while (timeElapsed < _duration)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

            if (_isEnd)
            {
                warningSubLeft.localScale = Vector2.Lerp(initalLeft, new Vector2(0.7f, 0.0f), time);
                warningSubRight.localScale = Vector2.Lerp(initalRight, new Vector2(0.7f, 0.0f), time);
                warningSubMiddle.localScale = Vector2.Lerp(initalMiddle, new Vector2(0.7f, 0.0f), time);
            }   // isEnd: true
            else
            {
                warningSubLeft.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalLeft, time);
                warningSubRight.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalRight, time);
                warningSubMiddle.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalMiddle, time);
            }   // isEnd: false

            yield return null;
        }

        if (_isEnd)
        {
            warningSubLeft.localScale = new Vector2(0.7f, 0.0f);
            warningSubRight.localScale = new Vector2(0.7f, 0.0f);
            warningSubMiddle.localScale = new Vector2(0.7f, 0.0f);
        }
        else
        {
            warningSubLeft.localScale = initalLeft;
            warningSubRight.localScale = initalRight;
            warningSubMiddle.localScale = initalMiddle;
        }
    }

    private IEnumerator NoticeScale(float _duration, bool _isEnd, bool _isText)
    {
        timeElapsed = 0.0f;

        if (_isText)
        {
            initalLeft = new Vector2(0.7f, 0.5f);
            initalMiddle = new Vector2(0.7f, 0.5f);
            initalRight = new Vector2(0.7f, 0.5f);
        }   // isText: true
        else
        {
            initalLeft = new Vector2(0.7f, 0.7f);
            initalMiddle = new Vector2(0.7f, 0.7f);
            initalRight = new Vector2(0.7f, 0.7f);
        }   // isText: false

        while (timeElapsed < _duration)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

            if (_isEnd)
            {
                noticeLeft.localScale = Vector2.Lerp(initalLeft, new Vector2(0.7f, 0.0f), time);
                noticeRight.localScale = Vector2.Lerp(initalRight, new Vector2(0.7f, 0.0f), time);
                noticeMiddle.localScale = Vector2.Lerp(initalMiddle, new Vector2(0.7f, 0.0f), time);
            }   // isEnd: true
            else
            {
                noticeLeft.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalLeft, time);
                noticeRight.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalRight, time);
                noticeMiddle.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalMiddle, time);
            }   // isEnd: false

            yield return null;
        }

        if (_isEnd)
        {
            noticeLeft.localScale = new Vector2(0.7f, 0.0f);
            noticeRight.localScale = new Vector2(0.7f, 0.0f);
            noticeMiddle.localScale = new Vector2(0.7f, 0.0f);
        }
        else
        {
            noticeLeft.localScale = initalLeft;
            noticeRight.localScale = initalRight;
            noticeMiddle.localScale = initalMiddle;
        }
    }

    private IEnumerator WarningSubMove(Vector2 _left, Vector2 _right, Vector2 _middle, float _duration)
    {
        timeElapsed = 0.0f;

        initalLeft = warningSubLeft.anchoredPosition;
        initalRight = warningSubRight.anchoredPosition;
        initalMiddle = warningSubMiddle.localScale;

        while (timeElapsed < _duration)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

            warningSubLeft.anchoredPosition = Vector2.Lerp(initalLeft, initalLeft + _left, time);
            warningSubRight.anchoredPosition = Vector2.Lerp(initalRight, initalRight + _right, time);
            warningSubMiddle.localScale = Vector2.Lerp(initalMiddle, initalMiddle + _middle, time);

            yield return null;
        }

        warningSubLeft.anchoredPosition = initalLeft + _left;
        warningSubRight.anchoredPosition = initalRight + _right;
        warningSubMiddle.localScale = initalMiddle + _middle;
    }

    private IEnumerator NoticeMove(Vector2 _left, Vector2 _right, Vector2 _middle, float _duration)
    {
        timeElapsed = 0.0f;

        initalLeft = noticeLeft.anchoredPosition;
        initalRight = noticeRight.anchoredPosition;
        initalMiddle = noticeMiddle.localScale;

        while (timeElapsed < _duration)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

            noticeLeft.anchoredPosition = Vector2.Lerp(initalLeft, initalLeft + _left, time);
            noticeRight.anchoredPosition = Vector2.Lerp(initalRight, initalRight + _right, time);
            noticeMiddle.localScale = Vector2.Lerp(initalMiddle, initalMiddle + _middle, time);

            yield return null;
        }

        noticeLeft.anchoredPosition = initalLeft + _left;
        noticeRight.anchoredPosition = initalRight + _right;
        noticeMiddle.localScale = initalMiddle + _middle;
    }

    private IEnumerator WarningSubText(float _duration, bool _isEnd, bool _isText)
    {
        timeElapsed = 0.0f;

        while (timeElapsed < _duration)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

            if (_isEnd)
            {
                if (_isText) { warningEndText.alpha = Mathf.Lerp(1.0f, 0.0f, time); }
                else { warningStartText.alpha = Mathf.Lerp(1.0f, 0.0f, time); }
            }
            else
            {
                if (_isText) { warningEndText.alpha = Mathf.Lerp(1.0f, 0.0f, time); }
                else { warningStartText.alpha = Mathf.Lerp(0.0f, 1.0f, time); }
            }

            yield return null;
        }

        if (_isEnd)
        {
            if (_isText) { warningEndText.alpha = 0.0f; }
            else { warningStartText.alpha = 0.0f; }
        }
        else
        {
            if (_isText) { warningEndText.alpha = 1.0f; }
            else { warningStartText.alpha = 1.0f; }
        }
    }

    private IEnumerator NoticeText(float _duration, bool _isEnd, bool _isText)
    {
        timeElapsed = 0.0f;

        while (timeElapsed < _duration)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

            if (_isEnd)
            {
                if (_isText) { noticeTextText.alpha = Mathf.Lerp(1.0f, 0.0f, time); }
                else { noticeTextCount.alpha = Mathf.Lerp(1.0f, 0.0f, time); }
            }
            else
            {
                if (_isText) { noticeTextText.alpha = Mathf.Lerp(0.0f, 1.0f, time); }
                else { noticeTextCount.alpha = Mathf.Lerp(0.0f, 1.0f, time); }
            }

            yield return null;
        }

        if (_isEnd)
        {
            if (_isText) { noticeTextText.alpha = 0.0f; }
            else { noticeTextCount.alpha = 0.0f; }
        }
        else
        {
            if (_isText) { noticeTextText.alpha = 1.0f; }
            else { noticeTextCount.alpha = 1.0f; }
        }
    }

    private IEnumerator GameOverText(float _duration, CanvasGroup _gameObject)
    {
        timeElapsed = 0.0f;

        while (timeElapsed < _duration)
        {
            timeElapsed += Time.deltaTime;

            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

            _gameObject.alpha = Mathf.Lerp(0.0f, 1.0f, time);

            yield return null;
        }
    }
}
