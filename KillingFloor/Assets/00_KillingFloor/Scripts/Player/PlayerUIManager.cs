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
    private static PlayerUIManager m_instance; // �̱����� �Ҵ�� ����

    public TMP_Text playerLevel;
    public TMP_Text hpText;         // ü�� ǥ��
    public TMP_Text shiedldText;    // �ǵ� ǥ��
    public TMP_Text ammoText;       // ź�� ǥ��
    public TMP_Text totalAmmoText;  // ���� ź��
    public TMP_Text grenadeText;    // ���� ����ź
    public TMP_Text coinText;       // ���� ��ȭ
    public TMP_Text weightText;     // ���� ����
    public Slider healSlider;        // �� �����̴�
    public GameObject equipUI;
    public GameObject shopUI;

    // ���� ����ȿ�� ���� ����
    private int coin;
    private int targetCoin;



    //JunOh
    public TMP_Text warningSubText;   // �˸� ����
    public TMP_Text noticeTextText;   // �˸� �ΰ� ����
    public TMP_Text noticeCountText;  // �˸� ���̺� ����
    public TMP_Text zombieCountText;  // ���� ��
    public TMP_Text timerCountText;   // Ÿ�̸�
    public TMP_Text zombieWaveText;   // ���� ���̺� ����

    public void Update()
    {
        CoinUpdate();
        SetNoticeWave();
        SetZombieWave();
    }

    // ü�� �ؽ�Ʈ ����
    public void SetLevel(float value)
    {
        playerLevel.text = string.Format("{0}", value);
    }
    public void SetHP(float value)
    {
        hpText.text = string.Format("{0}", value);
    }
    // �ǵ� �ؽ�Ʈ ����
    public void SetArmor(float value)
    {
        shiedldText.text = string.Format("{0}", value);
    }
    public void SetAmmo(float value)
    {
        if (value == 999)
        { ammoText.text = string.Format("��"); }
        else
            ammoText.text = string.Format("{0}", value);
    }
    public void SetRemainingAmmo(float value)
    {
        if (value == 999)
        { totalAmmoText.text = string.Format("��"); }
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

    // ���� ȹ��
    public void SetCoin(int value)
    {
        targetCoin = value;

    }
    // ���� ���� ������Ʈ
    public void CoinUpdate()
    {

        if (coin < targetCoin)
        {
            coin += Mathf.CeilToInt(1f * Time.deltaTime); // �ʴ� ���� ������Ʈ
            if (coin >= targetCoin)
            {
                coin = targetCoin; // ���� ���ο� �����ϸ� ����
            }
            coinText.text = string.Format("{0}", coin);
        }

        else
        {
            coin -= Mathf.CeilToInt(1f * Time.deltaTime); // �ʴ� ���� ������Ʈ
            if (coin <= targetCoin)
            {
                coin = targetCoin; // ���� ���ο� �����ϸ� ����
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