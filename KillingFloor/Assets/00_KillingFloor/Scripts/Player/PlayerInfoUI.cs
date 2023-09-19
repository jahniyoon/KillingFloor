using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PlayerInfoUI : MonoBehaviour
{
    public TMP_Text playerNickname;
    public TMP_Text playerLevel;
    public Slider armor;
    public Slider health;


    // Start is called before the first frame update
    void Start()
    {
        playerNickname.text = string.Format(PhotonNetwork.LocalPlayer.NickName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetArmor(float value)
    {
        armor.value = value;
    }
    public void SetHealth(float value)
    {
        health.value = value;
    }
    public void SetNickName(string name)
    {
        playerNickname.text = string.Format("{0}", name);
    }
    public void SetLevel(int level)
    {
        playerLevel.text = string.Format("{0}", level);
    }
}
