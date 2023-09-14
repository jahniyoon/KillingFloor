using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
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

    // ü�� �ؽ�Ʈ ����
    public void SetLevel(float value)
    {
        playerLevel.text = string.Format("{0}", value);
    }
    public void SetHP(float value)
    {
        hpText.text =string.Format("{0}", value) ;
    }
    // �ǵ� �ؽ�Ʈ ����
    public void SetShield(float value)
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
}
