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
    [SerializeField]
    private GameObject loadingHUD;
    [SerializeField]
    private GameObject playerHUD;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartCheck());
        }
    }

    private IEnumerator StartCheck()
    {
        while (!CheckIfAllPlayersLoaded())
        { yield return null; }

        GameManager.instance.isCheck = true;
        StartGame();
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

    private void StartGame()
    {
        photonView.RPC("MasterStart", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void MasterStart()
    {
        photonView.RPC("SyncStart", RpcTarget.All);
    }

    [PunRPC]
    private void SyncStart()
    {
        loadingHUD.SetActive(false);
        playerHUD.SetActive(true);
    }
}