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
//        if (Input.GetKey(KeyCode.V) && isCheck == false)
//        {
//            isCheck = true;
//            StartCoroutine(StartMove(new Vector2(-300, 0), new Vector2(300, 0), new Vector2(60, 0), 0.2f));
//        }
//        if (Input.GetKey(KeyCode.Z))
//        {
//            isCheck = false;
//        }
//    }

//    private IEnumerator CoroutineManager()
//    {
//        StartCoroutine(WarningSubScale(Vector2.zero, Vector2.zero, Vector2.zero, 0.1f, false));
//    }

//    private IEnumerator WarningSubScale(Vector2 _left, Vector2 _right, Vector2 _middle, float _duration, bool isEnd)
//    {
//        timeElapsed = 0.0f;

//        initalLeft = new Vector2(0.7f, 0.7f);
//        initalMiddle = new Vector2(0.7f, 0.7f);
//        initalRight = new Vector2(0.7f, 0.7f);

//        while (timeElapsed < _duration)
//        {
//            timeElapsed += Time.deltaTime;

//            float time = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(timeElapsed / _duration), 2);

//            if (isEnd)
//            {
//                warningSubLeft.localScale = Vector2.Lerp(initalLeft, _left, time);
//                warningSubRight.localScale = Vector2.Lerp(initalRight, _right, time);
//                warningSubMiddle.localScale = Vector2.Lerp(initalMiddle, _middle, time);
//            }
//            else
//            { 
//                warningSubLeft.localScale = Vector2.Lerp(_left, initalLeft, time);
//                warningSubRight.localScale = Vector2.Lerp(_right, initalRight, time);
//                warningSubMiddle.localScale = Vector2.Lerp(_middle, initalMiddle, time);
//            }

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

//            warningSubLeft.anchoredPosition = Vector2.Lerp(initalLeft, initalLeft + _leftPos, time);
//            warningSubRight.anchoredPosition = Vector2.Lerp(initalRight, initalRight + _rightPos, time);
//            warningSubMiddle.localScale = Vector2.Lerp(initalMiddle, initalMiddle + _middleScale, time);

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
