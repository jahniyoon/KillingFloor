using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Slider healSlider;        // 힐 슬라이더
    public GameObject equipUI;
    public GameObject shopUI;

    // 코인 증가효과 계산용 변수
    private int coin;
    private int targetCoin;

    //JunOh
    public TMP_Text NoticeText;       // 알림 내용
    public TMP_Text NoticeWaveText;   // 알림 웨이브 정보
    public TMP_Text ZombieCountText;  // 좀비 수
    public TMP_Text ZombieWaveText;   // 좀비 웨이브 정보
                                      //JunOh
    public void Update()
    {
        CoinUpdate();
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
        if(value == 999)
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
        // 코인이 올라갈 때만 업데이트
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

    //JunOh
    public void SetNotice(string value)
    {
        NoticeText.text = string.Format("{0}", value);
    }
    public void SetNoticeWave(float stageValue, float waveValue)
    {
        NoticeWaveText.text = string.Format("[ {0}/ {1} ]", stageValue, waveValue);
    }

    public void SetZombieCount(float countValue, float minValue, float secValue, bool isZombie)
    {
        if (isZombie)
        {
            ZombieCountText.text = string.Format("{0}", countValue);
        }
        else
        {
            ZombieCountText.text = string.Format("{0:00}:{1:00}", minValue, secValue);
        }
    }

    public void SetZombieWave(float stageValue, float waveValue)
    {
        ZombieWaveText.text = string.Format("{0}/ {1}", stageValue, waveValue);
    }
    //JunOh
}
