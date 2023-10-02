using Photon.Pun;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;
using WebSocketSharp;

public class PlayerInfoUI : MonoBehaviourPun
{
    public enum State { Live, Die }
    public State state;

    public string nickName;
    public string level;
    public string playerClass;
    public int playerSkin;
    public PlayerHealth m_player;

    public TMP_Text playerNickname;
    public TMP_Text playerLevel;
    public TMP_Text playerClassText;
    public GameObject[] classIcon;

    public Slider armor;
    public Slider health;
    public Slider exp;

    public TMP_Text healtHUD;
    public TMP_Text armorHUD;
    public TMP_Text coinHUD;
    public TMP_Text levelHUD;

    public float playerHealth;  // 블러드스크린에 영향을 주기위한 플레이어의 체력
    public Image bloodScreen;   // 피 데미지 스크린
    public Image poisonScreen;  // 독 데미지 스크린
    public float bloodScreenValue;
    public float poisonScreenValue;

    public bool cameraShakeTrigger;

    // 코인 증가효과 계산용 변수
    private int coin;
    private int targetCoin;

    // 플레이어 스킨 변경
    public GameObject[] Lod;
    public Material[] changeMat;

    // Start is called before the first frame update
    void Start()
    {

        if(photonView.IsMine)
        {
            GetPlayerData();
            healtHUD = PlayerUIManager.instance.hpText;
            armorHUD = PlayerUIManager.instance.shiedldText;
            coinHUD = PlayerUIManager.instance.coinText;
            levelHUD = PlayerUIManager.instance.playerLevel;
            exp = PlayerUIManager.instance.expSlider;
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
        playerClass = GameManager.instance.playerClass;
        playerSkin = GameManager.instance.playerSkin;

        photonView.RPC("DataProcessOnServer", RpcTarget.All, nickName, level, playerClass);
        PlayerUIManager.instance.SetLevel(level);
        PlayerUIManager.instance.SetClass(playerClass);

        ChangeMat(playerSkin);
    }

    [PunRPC]
    public void DataProcessOnServer(string _nickName, string _level, string _class)
    {
        playerNickname.text = _nickName;
        playerLevel.text = _level;
        playerClassText.text = _class;

        switch (_class)
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
   
    public void SetArmor(float value)
    {
        armor.value = value;
        if (photonView.IsMine && armorHUD != null)
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
    public void SetLevel(int _level)
    {
        playerLevel.text = string.Format("{0}", _level);
        level = string.Format("{0}", _level);
        levelHUD.text = string.Format("{0}", _level);
    }
    public void SetExp(int value)
    {
        exp.value = value;
    }
 

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
                coinHUD.text = string.Format("{0}", coin);
            }

            else
            {
                coin -= Mathf.CeilToInt(1f * Time.deltaTime); // 초당 코인 업데이트
                if (coin <= targetCoin)
                {
                    coin = targetCoin; // 현재 코인에 도달하면 멈춤
                }
                coinHUD.text = string.Format("{0}", coin);
            }
    }


    // 스크린 데미지 업데이트
    public void DamageScreenUpdate()
    {
        // 독상태 확인
        if (0 < poisonScreenValue)
        {
            bloodScreenValue = 0;
        }


        // 블러드 스크린의 값이 있을 경우 0이 될때까지 실행
        if (0 < bloodScreenValue)
        {
            bloodScreenValue -= Mathf.CeilToInt(1 * Time.deltaTime);
            bloodScreen.color = new Color(255, 255, 255, bloodScreenValue / 1000);
        }
        // 블러드 스크린의 값이 있을 경우 0이 될때까지 실행
        if (0 < poisonScreenValue)
        {
            poisonScreenValue -= Mathf.CeilToInt(1 * Time.deltaTime);
            poisonScreen.color = new Color(255, 255, 255, poisonScreenValue / 100);
        }
    }
    // 블러드 스크린의 값 조정
    public void SetBloodScreen(float _health)
    {
        // 체력이 낮을 경우 더 붉어지게 하기 위한 값
        // 체력이 100이면 변함없음
        float newHealth = (-1 * _health + 100);

        bloodScreenValue += 120 + newHealth;
        if (900 < bloodScreenValue)
        { bloodScreenValue = 900; }

        cameraShakeTrigger = true;  // 카메라 흔들기를 위한 Bool
    }
    public void ResetScreen()
    {
        bloodScreenValue = 0f;
        poisonScreenValue = 0;
        bloodScreen.color = new Color(255, 255, 255, 0);
        poisonScreen.color = new Color(255, 255, 255,0);
    }
    // 포이즌 스크린의 값 조정
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

    public void ChangeMat(int index)
    {

        for (int i = 0; i < Lod.Length; i++)
        {
            Lod[i].GetComponent<SkinnedMeshRenderer>().material = changeMat[index];
        }
    }
}
