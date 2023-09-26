using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : MonoBehaviourPun
{
    private bool isCheck = false;

    private void Start()
    {
        asyncLoadScene("Main");
    }

    public void asyncLoadScene(string name)
    {
        // �񵿱� �� �ε� ����
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        asyncLoad.allowSceneActivation = false; // �� �ε� �Ϸ� �� �ڵ� Ȱ��ȭ ����

        // �ε��� �Ϸ�� ������ ���
        StartCoroutine(WaitForSceneLoad(asyncLoad));
    }

    private IEnumerator WaitForSceneLoad(AsyncOperation asyncLoad)
    {
        while (!asyncLoad.isDone)
        {
            // �ε� ���� ��Ȳ�� ǥ���ϰų� �߰� �۾� ���� ����
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9f�� �ε��� 90%���� �Ϸ�Ǿ��ٴ� �ǹ�

            // ������ �����ϸ� �� Ȱ��ȭ
            if (progress >= 1.0f)
            {
                if (CheckIfAllPlayersLoaded())
                {
                    if (PhotonNetwork.IsMasterClient) { Load(); }

                    while (!isCheck) { yield return null; }

                    asyncLoad.allowSceneActivation = true;
                }
            }

            // ���� �����ӱ��� ���
            yield return null;
        }
    }

    private bool CheckIfAllPlayersLoaded()
    {
        // ��� �÷��̾��� �ε� ���¸� Ȯ��
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object loadingCompleteObj;
            if (player.CustomProperties.TryGetValue("LoadingComplete", out loadingCompleteObj))
            {
                bool loadingComplete = (bool)loadingCompleteObj;
                if (!loadingComplete)
                {
                    // �ε��� �Ϸ���� ���� �÷��̾ ������ false ��ȯ
                    return false;
                }
            }
        }

        // ��� �÷��̾ �ε��� �Ϸ������� true ��ȯ
        return true;
    }

    private void Load()
    {
        photonView.RPC("MasterLoad", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void MasterLoad()
    {
        photonView.RPC("SyncLoad", RpcTarget.All);
    }

    [PunRPC]
    private void SyncLoad()
    {
        isCheck = true;
    }
}