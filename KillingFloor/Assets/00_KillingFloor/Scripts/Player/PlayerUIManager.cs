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
    [Header("Player")]
    public TMP_Text playerLevel;
    public TMP_Text hpText;         // 체력 표시
    public TMP_Text shiedldText;    // 실드 표시
    public TMP_Text ammoText;       // 탄약 표시
    public TMP_Text totalAmmoText;  // 남은 탄약
    public TMP_Text grenadeText;    // 남은 수류탄
    public TMP_Text coinText;       // 현재 재화
    public TMP_Text weightText;     // 현재 무게       
    public GameObject[] classIcon;  // 클래스 아이콘
    public Slider expSlider;        // 경험치

    // 코인 증가효과 계산용 변수
    private int coin;
    private int targetCoin;

    [Header("Shop")]
    public TMP_Text shopDistance;   // 상점까지 거리
    public Slider shopUpRotation;     // 상점 방향
    public Slider shopDownRotation;     // 상점 방향
    public Slider healSlider;       // 힐 슬라이더
    public GameObject equipUI;      // 상호작용 UI
    public GameObject shopUI;       // 상점 상호작용 UI
    public GameObject pauseUI;       // 포즈 UI

    [Header("BloodScreen")]
    public float playerHealth;  // 블러드스크린에 영향을 주기위한 플레이어의 체력
    public Image bloodScreen;   // 피 데미지 스크린
    public Image poisonScreen;  // 독 데미지 스크린
    public float bloodScreenValue;
    public float poisonScreenValue;
    public bool isPoison; // 현재 포이즌 상태인지 확인

    [Header("Pause")]
    public Slider mouseSensitive;
    public TMP_Text mouseSensitiveValue;
    public bool isShopState;
    public bool isPauseState;

    public GameObject gameOverUI;
    public GameObject leaveButton;

    [Header("Wave")]
    //JunOh
    public Text warningEndText;   // 웨이브 종료 내용
    public Text warningStartText; // 웨이브 시작 내용
    public Text noticeTextText;   // 알림 로고 정보
    public TMP_Text noticeCountText;  // 알림 웨이브 정보
    public TMP_Text zombieCountText;  // 좀비 수
    public TMP_Text timerCountText;   // 타이머
    public TMP_Text zombieWaveText;   // 좀비 웨이브 정보
    public GameObject CountUI;
    public GameObject TimerUI;

    public void Update()
    {
        SetNoticeWave();
        SetZombieWave();
        Pause();
        DamageScreenUpdate();
    }

    // 체력 텍스트 갱신
    public void SetLevel(string value)
    {
        playerLevel.text = value;
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
    public void SetClass(string playerClass)
    {
        switch (playerClass)
        {
            case "Commando":
                classIcon[0].gameObject.SetActive(true); 
                classIcon[1].gameObject.SetActive(false); 
                break;
            case "Demolitionist":
                classIcon[0].gameObject.SetActive(false);
                classIcon[1].gameObject.SetActive(true);
                break;
        }
    }
    public void SetExp(int value)
    {
        expSlider.value = value;
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
    public void SetShopDistance(float value)
    {
        shopDistance.text = string.Format("{0}M", value);
    }
    public void SetShopRotation(float value, bool isUp)
    {  
        shopUpRotation.value = value;
        shopDownRotation.value = value;

        // 위 아래 감지 
        shopUpRotation.gameObject.SetActive(isUp);
        shopDownRotation.gameObject.SetActive(!isUp);

    }

    // 스크린 데미지 업데이트
    public void DamageScreenUpdate()
    {
        // 블러드 스크린의 값이 있을 경우 0이 될때까지 실행
        if (0 < bloodScreenValue)
        {
            bloodScreenValue -= Mathf.CeilToInt(1 * Time.deltaTime);
            bloodScreen.color = new Color(255, 255, 255, bloodScreenValue/1000);
        }
        // 블러드 스크린의 값이 있을 경우 0이 될때까지 실행
        if (0 < poisonScreenValue)
        {
            poisonScreenValue -= Mathf.CeilToInt(1 * Time.deltaTime);
            poisonScreen.color = new Color(255, 255, 255, bloodScreenValue / 100);
        }
    }
    // 블러드 스크린의 값 조정
    public void SetBloodScreen(float _health)
    {
        // 체력이 낮을 경우 더 붉어지게 하기 위한 값
        // 체력이 100이면 변함없음
        float newHealth = (-1*_health+100);

        bloodScreenValue += 200 + newHealth;
        if(1000f < bloodScreenValue)
        { bloodScreenValue = 1000f;}
    }
    // 포이즌 스크린의 값 조정
    public void SetPoisonScreen()
    {
        poisonScreenValue += 200;
    }

    // 포즈 업데이트
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
    // 포즈 켜기
    public void OnPause()
    {
        isPauseState = true;
    }
    // 포즈 종료
    public void OffPause()
    {
        isPauseState = false;
        pauseUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.instance.inputLock = false;  // 인풋락도 풀어주기

    }
    // 서버 나가기 버튼
    public void LeaveRoomButton()
    {
        GameManager.instance.LeaveServer();
    }
    // 마우스 센서티브 업데이트
    public void MouseSensitiveUpdate()
    {
        mouseSensitiveValue.text = string.Format("{0}", Mathf.FloorToInt(mouseSensitive.value));
    }



    //JunOh

    public void SetEndNotice(string value)
    {
        warningEndText.text = string.Format("{0}", value);
    }

    public void SetStartNotice(string value)
    {
        warningStartText.text = string.Format("{0}", value);
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