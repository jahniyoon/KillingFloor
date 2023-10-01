using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGamaManager : MonoBehaviour
{

    // 로컬플레이어 레벨 +1 Test 메서드
    public void OnLvUpBtn()
    {
        // 버튼을 누를시 localPlayerLv 값 재할당
        int lvUp = int.Parse(NetworkManager.net_instance.localPlayerLv) + 1;
        NetworkManager.net_instance.localPlayerLv = lvUp.ToString();

        Debug.Log("더하기 1 했을 때: " + NetworkManager.net_instance.localPlayerLv);

        // 데이터에 변경된 레벨 서버에 저장하기 위해서 꼭 필요
        NetworkManager.net_instance.SetData(NetworkManager.net_instance.localPlayerLv);
    }
    // 로컬플레이어 레벨 -1 Test 메서드
    public void OnLvDownBtn()
    {
        int lvDown = int.Parse(NetworkManager.net_instance.localPlayerLv) - 1;
        NetworkManager.net_instance.localPlayerLv = lvDown.ToString();

        Debug.Log("빼기 1 했을 때: " + NetworkManager.net_instance.localPlayerLv);

        NetworkManager.net_instance.SetData(NetworkManager.net_instance.localPlayerLv);
    }
}
