using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGamaManager : MonoBehaviour
{

    // �����÷��̾� ���� +1 Test �޼���
    public void OnLvUpBtn()
    {
        int lvUp = int.Parse(NetworkManager.instance.localPlayerLv) + 1;
        NetworkManager.instance.localPlayerLv = lvUp.ToString();
        //string localPlayerLvUp = lvUp.ToString();

        Debug.Log("���ϱ� 1 ���� ��: " + NetworkManager.instance.localPlayerLv);

        NetworkManager.instance.SetData(NetworkManager.instance.localPlayerLv);
    }
    // �����÷��̾� ���� -1 Test �޼���
    public void OnLvDownBtn()
    {
        int lvDown = int.Parse(NetworkManager.instance.localPlayerLv) - 1;
        NetworkManager.instance.localPlayerLv = lvDown.ToString();
        //string localPlayerLvUp = lvUp.ToString();

        Debug.Log("���� 1 ���� ��: " + NetworkManager.instance.localPlayerLv);

        NetworkManager.instance.SetData(NetworkManager.instance.localPlayerLv);
    }
}
