using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Junoh 추가
    public int round = 1;       // 현재 라운드
    public int player = 4;      // 플레이어 인원 수
    public int difficulty = 0;  // 난이도 0: 보통 1: 어려움 2: 지옥
    // Junoh 추가

    public void Awake()
    {
        if(instance == null)
        { instance = this; }
        else
        { GlobalFunc.LogWarning("씬에 두 개 이상의 게임 매니저가 존재합니다."); }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
