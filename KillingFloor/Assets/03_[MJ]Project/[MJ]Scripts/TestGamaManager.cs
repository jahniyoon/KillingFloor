using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGamaManager : MonoBehaviour
{

    // 로컬플레이어 레벨 +1 Test 메서드
    public void OnLvUpBtn()
    {
        int lvUp = int.Parse(NetworkManager.instance.localPlayerLv) + 1;
        NetworkManager.instance.localPlayerLv = lvUp.ToString();
        //string localPlayerLvUp = lvUp.ToString();

        Debug.Log("더하기 1 했을 때: " + NetworkManager.instance.localPlayerLv);

        NetworkManager.instance.SetData(NetworkManager.instance.localPlayerLv);
    }
    // 로컬플레이어 레벨 -1 Test 메서드
    public void OnLvDownBtn()
    {
        int lvDown = int.Parse(NetworkManager.instance.localPlayerLv) - 1;
        NetworkManager.instance.localPlayerLv = lvDown.ToString();
        //string localPlayerLvUp = lvUp.ToString();

        Debug.Log("빼기 1 했을 때: " + NetworkManager.instance.localPlayerLv);

        NetworkManager.instance.SetData(NetworkManager.instance.localPlayerLv);
    }
}
