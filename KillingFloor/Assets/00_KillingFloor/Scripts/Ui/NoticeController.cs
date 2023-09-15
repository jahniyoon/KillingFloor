//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;

//public class NoticeController : MonoBehaviour
//{
//    public RectTransform warningMain;

//    public RectTransform warningSubLeft;
//    public RectTransform warningSubMiddle;
//    public RectTransform warningSubRight;

//    public RectTransform noticeLeft;
//    public RectTransform noticeMiddle;
//    public RectTransform noticeRight;

//    private Vector2 initalLeft;
//    private Vector2 initalRight;
//    private Vector2 initalMiddle;

//    private float timeElapsed = 0.0f;

//    private bool isCoroutine = false;
//    private bool isCheck = false;
//    public bool isText = true;

//    private void Awake()
//    {

//    }

//    private void OnEnable()
//    {

//    }

//    private void Start()
//    {
//        //StartCoroutine(StartMotion());
//    }

//    private void Update()
//    {
//        if (isCoroutine == false)
//        {
//            if (Input.GetKey(KeyCode.V) && isCheck == false)
//            {
//                isCheck = true;
//                StartCoroutine(CoroutineManager());
//            }
//            if (Input.GetKey(KeyCode.Z))
//            {
//                isCheck = false;
//            }
//        }
//    }

//    private IEnumerator CoroutineManager()
//    {
//        isCoroutine = true;
//        StartCoroutine(WarningSubScale(0.1f, false));
//        yield return new WaitForSeconds(1.0f);
//        StartCoroutine(NoticeScale(0.1f, false, false));
//        yield return new WaitForSeconds(1.0f);
//        StartCoroutine(NoticeScale(0.1f, true, false));
//        yield return new WaitForSeconds(1.0f);
//        StartCoroutine(NoticeScale(0.1f, false, true));
//        yield return new WaitForSeconds(1.0f);
//        StartCoroutine(NoticeScale(0.1f, true, true));
//        yield return new WaitForSeconds(1.0f);
//        StartCoroutine(WarningSubScale(0.1f, true));

//        isCoroutine = false;
//        yield break;
//    }

//    private IEnumerator WarningSubScale(float _duration, bool _isEnd)
//    {
//        timeElapsed = 0.0f;

//        initalLeft = new Vector2(0.7f, 0.7f);
//        initalMiddle = new Vector2(0.7f, 0.7f);
//        initalRight = new Vector2(0.7f, 0.7f);

//        while (timeElapsed < _duration)
//        {
//            timeElapsed += Time.deltaTime;

//            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

//            if (_isEnd)
//            {
//                warningSubLeft.localScale = Vector2.Lerp(initalLeft, new Vector2(0.7f, 0.0f), time);
//                warningSubRight.localScale = Vector2.Lerp(initalRight, new Vector2(0.7f, 0.0f), time);
//                warningSubMiddle.localScale = Vector2.Lerp(initalMiddle, new Vector2(0.7f, 0.0f), time);
//            }   // isEnd: true
//            else
//            {
//                warningSubLeft.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalLeft, time);
//                warningSubRight.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalRight, time);
//                warningSubMiddle.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalMiddle, time);
//            }   // isEnd: false

//            yield return null;
//        }
//    }

//    private IEnumerator NoticeScale(float _duration, bool _isEnd, bool _isText)
//    {
//        timeElapsed = 0.0f;

//        if (_isText)
//        {
//            initalLeft = new Vector2(0.7f, 0.5f);
//            initalMiddle = new Vector2(0.7f, 0.5f);
//            initalRight = new Vector2(0.7f, 0.5f);
//        }   // isText: true
//        else
//        {
//            initalLeft = new Vector2(0.7f, 0.7f);
//            initalMiddle = new Vector2(0.7f, 0.7f);
//            initalRight = new Vector2(0.7f, 0.7f);
//        }   // isText: false

//        while (timeElapsed < _duration)
//        {
//            timeElapsed += Time.deltaTime;

//            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

//            if (_isEnd)
//            {
//                noticeLeft.localScale = Vector2.Lerp(initalLeft, new Vector2(0.7f, 0.0f), time);
//                noticeRight.localScale = Vector2.Lerp(initalRight, new Vector2(0.7f, 0.0f), time);
//                noticeMiddle.localScale = Vector2.Lerp(initalMiddle, new Vector2(0.7f, 0.0f), time);
//            }   // isEnd: true
//            else
//            {
//                noticeLeft.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalLeft, time);
//                noticeRight.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalRight, time);
//                noticeMiddle.localScale = Vector2.Lerp(new Vector2(0.7f, 0.0f), initalMiddle, time);
//            }   // isEnd: false

//            yield return null;
//        }
//    }

//    private IEnumerator StartMove(Vector2 _left, Vector2 _right, Vector2 _middle, float _duration)
//    {
//        timeElapsed = 0.0f;

//        initalLeft = warningSubLeft.anchoredPosition;
//        initalRight = warningSubRight.anchoredPosition;
//        initalMiddle = warningSubMiddle.localScale;

//        while (timeElapsed < _duration)
//        {
//            timeElapsed += Time.deltaTime;

//            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

//            warningSubLeft.anchoredPosition = Vector2.Lerp(initalLeft, initalLeft + _left, time);
//            warningSubRight.anchoredPosition = Vector2.Lerp(initalRight, initalRight + _right, time);
//            warningSubMiddle.localScale = Vector2.Lerp(initalMiddle, initalMiddle + _middle, time);

//            yield return null;
//        }

//        timeElapsed = 0.0f;

//        ScaleChange();
//    }
//    private void ScaleChange()
//    {
//        if (isText)
//        {
//            noticeLeft.localScale = new Vector2(0.7f, 0.8f);
//            noticeMiddle.localScale = new Vector2(0.7f, 0.8f);
//            noticeRight.localScale = new Vector2(0.7f, 0.8f);
//        }
//        else
//        {
//            noticeLeft.localScale = new Vector2(0.7f, 0.5f);
//            noticeMiddle.localScale = new Vector2(0.7f, 0.5f);
//            noticeRight.localScale = new Vector2(0.7f, 0.5f);
//        }
//    }
//}
