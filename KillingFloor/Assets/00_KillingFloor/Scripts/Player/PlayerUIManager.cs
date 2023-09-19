using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.Rendering.DebugUI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<PlayerUIManager>();
            }

            return m_instance;
        }
    }
    private static PlayerUIManager m_instance; // 싱글톤이 할당될 변수

    public TMP_Text playerLevel;
    public TMP_Text hpText;         // 체력 표시
    public TMP_Text shiedldText;    // 실드 표시
    public TMP_Text ammoText;       // 탄약 표시
    public TMP_Text totalAmmoText;  // 남은 탄약
    public TMP_Text grenadeText;    // 남은 수류탄
    public TMP_Text coinText;       // 현재 재화
    public TMP_Text weightText;     // 현재 무게
    public Slider healSlider;       // 힐 슬라이더
    public GameObject equipUI;      // 상호작용 UI
    public GameObject shopUI;       // 상점 상호작용 UI
    public GameObject pauseUI;       // 포즈 UI
    public Slider mouseSensitive;
    public TMP_Text mouseSensitiveValue;
    public bool isShopState;
    public bool isPauseState;

    // 코인 증가효과 계산용 변수
    private int coin;
    private int targetCoin;



    //JunOh
    public TMP_Text warningSubText;   // 알림 내용
    public TMP_Text noticeTextText;   // 알림 로고 정보
    public TMP_Text noticeCountText;  // 알림 웨이브 정보
    public TMP_Text zombieCountText;  // 좀비 수
    public TMP_Text timerCountText;   // 타이머
    public TMP_Text zombieWaveText;   // 좀비 웨이브 정보
    public GameObject CountUI;
    public GameObject TimerUI;

    public void Update()
    {
        CoinUpdate();
        SetNoticeWave();
        SetZombieWave();
        Pause();
    }

    // 체력 텍스트 갱신
    public void SetLevel(float value)
    {
        playerLevel.text = string.Format("{0}", value);
    }
    public void SetHP(float value)
    {
        hpText.text = string.Format("{0}", value);
    }
    // 실드 텍스트 갱신
    public void SetArmor(float value)
    {
        shiedldText.text = string.Format("{0}", value);
    }
    public void SetAmmo(float value)
    {
        if (value == 999)
        { ammoText.text = string.Format("∞"); }
        else
            ammoText.text = string.Format("{0}", value);
    }
    public void SetRemainingAmmo(float value)
    {
        if (value == 999)
        { totalAmmoText.text = string.Format("∞"); }
        else
            totalAmmoText.text = string.Format("{0}", value);
    }
    public void SetGrenade(float value)
    {
        grenadeText.text = string.Format("{0}", value);
    }
    public void SetHeal(float value)
    {
        healSlider.value = value;
    }
    // 코인 획득
    public void SetCoin(int value)
    {
        targetCoin = value;

    }
    // 코인 증가 업데이트
    public void CoinUpdate()
    {

        if (coin < targetCoin)
        {
            coin += Mathf.CeilToInt(1f * Time.deltaTime); // 초당 코인 업데이트
            if (coin >= targetCoin)
            {
                coin = targetCoin; // 현재 코인에 도달하면 멈춤
            }
            coinText.text = string.Format("{0}", coin);
        }

        else
        {
            coin -= Mathf.CeilToInt(1f * Time.deltaTime); // 초당 코인 업데이트
            if (coin <= targetCoin)
            {
                coin = targetCoin; // 현재 코인에 도달하면 멈춤
            }
            coinText.text = string.Format("{0}", coin);
        }


    }
    public void SetWeight(float value)
    {
        weightText.text = string.Format("{0}", value);
    }

    // 포즈
    public void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isShopState && !isPauseState)
        {
            isPauseState = true;
            pauseUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.instance.inputLock = true;  // 인풋락도 걸어주기
        }
        // 상점 닫기 ESC
        else if (Input.GetKeyDown(KeyCode.Escape) && isPauseState)
        {
            OffPause();
        }
        MouseSensitiveUpdate();

    }
    public void OnPause()
    {
        isPauseState = true;
    }
    public void OffPause()
    {
        isPauseState = false;
        pauseUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.instance.inputLock = false;  // 인풋락도 풀어주기

    }
    public void LeaveRoomButton()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.LeaveRoom();
    }
    public void MouseSensitiveUpdate()
    {
        mouseSensitiveValue.text = string.Format("{0}", Mathf.FloorToInt(mouseSensitive.value));
    }



    //JunOh
    public void SetNotice(string value)
    {
        warningSubText.text = string.Format("{0}", value);
    }
    public void SetNoticeWave()
    {
        noticeCountText.text = string.Format("[ {0}/ {1} ]", GameManager.instance.round, GameManager.instance.wave);
    }

    public void SetNoticeLogo(string noticeTextValue)
    {
        noticeTextText.text = string.Format("{0}", noticeTextValue);
    }

    public void SetZombieCount(float countValue)
    {
        zombieCountText.text = string.Format("{0}", countValue);
    }

    public void SetTimerCount(int value)
    {
        timerCountText.text = string.Format("{0}:{1:D2}", value / 60, value % 60);
    }

    public void SetZombieWave()
    {
        zombieWaveText.text = string.Format("{0}/ {1}", GameManager.instance.round, GameManager.instance.wave);
    }
    //JunOh
}