using Photon.Pun;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerInfoUI : MonoBehaviourPun
{
    public enum State { Live, Die }
    public State state;

    public PlayerHealth m_player;
    public TMP_Text playerNickname;
    public string nickName;
    public TMP_Text playerLevel;
    public string level;
    public TMP_Text playerClass;
    public Slider armor;
    public Slider health;
    public TMP_Text healtHUD;
    public TMP_Text armorHUD;
    public TMP_Text coinHUD;

    public float playerHealth;  // ���彺ũ���� ������ �ֱ����� �÷��̾��� ü��
    public Image bloodScreen;   // �� ������ ��ũ��
    public Image poisonScreen;  // �� ������ ��ũ��
    public float bloodScreenValue;
    public float poisonScreenValue;

    public bool cameraShakeTrigger;

    // ���� ����ȿ�� ���� ����
    private int coin;
    private int targetCoin;

    // Start is called before the first frame update
    void Start()
    {

        if(photonView.IsMine)
        {
            GetPlayerData();
            healtHUD = PlayerUIManager.instance.hpText;
            armorHUD = PlayerUIManager.instance.shiedldText;
            coinHUD = PlayerUIManager.instance.coinText;
            bloodScreen = PlayerUIManager.instance.bloodScreen;
            poisonScreen = PlayerUIManager.instance.poisonScreen;   
        }
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            CoinUpdate();
            DamageScreenUpdate();
            PlayerCameraShake();
            PlayerState();
        }
    }

    public void GetPlayerData()
    {
        nickName = GameManager.instance.playerNickName;
        level = GameManager.instance.playerLevel;
        playerClass.text = GameManager.instance.playerClass;

        photonView.RPC("DataProcessOnServer", RpcTarget.All, nickName, level);
        PlayerUIManager.instance.SetLevel(level);
    }

    [PunRPC]
    public void DataProcessOnServer(string _nickName, string _level)
    {
        playerNickname.text = _nickName;
        playerLevel.text = _level;
    }
   
    public void SetArmor(float value)
    {
        armor.value = value;
        if (photonView.IsMine)
        { armorHUD.text = string.Format("{0}", value); }
    }
    public void SetHealth(float value)
    {
        health.value = value;
        if (photonView.IsMine && healtHUD != null)
        { healtHUD.text = string.Format("{0}", value); }
    }
    public void SetNickName(string name)
    {
        playerNickname.text = string.Format("{0}", name);
    }
    public void SetLevel(int level)
    {
        playerLevel.text = string.Format("{0}", level);
    }


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
                coinHUD.text = string.Format("{0}", coin);
            }

            else
            {
                coin -= Mathf.CeilToInt(1f * Time.deltaTime); // �ʴ� ���� ������Ʈ
                if (coin <= targetCoin)
                {
                    coin = targetCoin; // ���� ���ο� �����ϸ� ����
                }
                coinHUD.text = string.Format("{0}", coin);
            }
    }


    // ��ũ�� ������ ������Ʈ
    public void DamageScreenUpdate()
    {
        // ������ Ȯ��
        if (0 < poisonScreenValue)
        {
            bloodScreenValue = 0;
        }


        // ���� ��ũ���� ���� ���� ��� 0�� �ɶ����� ����
        if (0 < bloodScreenValue)
        {
            bloodScreenValue -= Mathf.CeilToInt(1 * Time.deltaTime);
            bloodScreen.color = new Color(255, 255, 255, bloodScreenValue / 1000);
        }
        // ���� ��ũ���� ���� ���� ��� 0�� �ɶ����� ����
        if (0 < poisonScreenValue)
        {
            poisonScreenValue -= Mathf.CeilToInt(1 * Time.deltaTime);
            poisonScreen.color = new Color(255, 255, 255, poisonScreenValue / 100);
        }
    }
    // ���� ��ũ���� �� ����
    public void SetBloodScreen(float _health)
    {
        // ü���� ���� ��� �� �Ӿ����� �ϱ� ���� ��
        // ü���� 100�̸� ���Ծ���
        float newHealth = (-1 * _health + 100);

        bloodScreenValue += 200 + newHealth;
        if (900 < bloodScreenValue)
        { bloodScreenValue = 900; }

        cameraShakeTrigger = true;  // ī�޶� ���⸦ ���� Bool
    }
    public void ResetScreen()
    {
        bloodScreenValue = 0f;
        poisonScreenValue = 0;
        bloodScreen.color = new Color(255, 255, 255, 0);
        poisonScreen.color = new Color(255, 255, 255,0);
    }
    // ������ ��ũ���� �� ����
    public void SetPoisonScreen()
    {
        bloodScreenValue = 0f;
        bloodScreen.color = new Color(255, 255, 255, 0);

        poisonScreenValue += 200;
    }
    public void PlayerCameraShake()
    {
        if (cameraShakeTrigger)
        {
            cameraShakeTrigger = false;
            PlayerFireCameraShake.Invoke();
        }
    }
    public void PlayerState()
    {
        if(state == State.Die)
        {
            GameManager.instance.inputLock = true;
        }
        if (state == State.Live)
        {
            GameManager.instance.inputLock = false;
        }
    }
}
