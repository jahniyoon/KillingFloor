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
    public Slider healSlider;        // �� �����̴�
    public GameObject equipUI;
    public GameObject shopUI;
    public GameObject shopOpenUI;

    //JunOh
    public TMP_Text NoticeText;       // �˸� ����
    public TMP_Text NoticeWaveText;   // �˸� ���̺� ����
    public TMP_Text ZombieCountText;  // ���� ��
    public TMP_Text ZombieWaveText;   // ���� ���̺� ����
    //JunOh

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
        if(value == 999)
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
