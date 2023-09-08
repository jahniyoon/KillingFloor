using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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

    public TMP_Text hpText; // ź�� ǥ�ÿ� �ؽ�Ʈ


    // ���� �ؽ�Ʈ ����
    public void UpdateHPText(float newHP)
    {
        hpText.text = "HP : " + newHP;
    }


}
