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

    public int playerCount;
    public int photonCount;
    public bool isCheck = false;

    private void Start()
    {
        GameManager.instance.inputLock = true;
    }
    private void Update()
    {
        if (!isCheck)
        {
            PlayerCheck();
        }
    }
    public void PlayerCheck()
    {
        playerCount = GameManager.instance.targetPlayer.Length;
        photonCount = PhotonNetwork.CurrentRoom.PlayerCount;
        // 포톤 룸의 총 플레이어가 메인씬 안의 사람들과 같은 경우
        if (PhotonNetwork.CurrentRoom.PlayerCount == GameManager.instance.targetPlayer.Length)
        {
            GameManager.instance.inputLock = false;

            if (PhotonNetwork.IsMasterClient)
            {
                if (!isCheck)
                {
                    isCheck = true;
                    GameManager.instance.isWave = true;

                    StartGame();
                }
            }
        }
    }
    private IEnumerator StartCheck()
    {
        Debug.Log(GameManager.instance.targetPlayer.Length);
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);

        while (PhotonNetwork.CurrentRoom.PlayerCount != GameManager.instance.targetPlayer.Length)
        { yield return null; }
        while (!CheckIfAllPlayersLoaded())
        { yield return null; }

        GameManager.instance.isCheck = true;
        GameManager.instance.WaveStart();

        StartGame();

    }

    private bool CheckIfAllPlayersLoaded()
    {
        // 모든 플레이어의 로딩 상태를 확인
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object loadingCompleteObj;
            if (player.CustomProperties.TryGetValue("LoadingComplete", out loadingCompleteObj))
            {
                bool loadingComplete = (bool)loadingCompleteObj;
                if (!loadingComplete)
                {
                    // 로딩이 완료되지 않은 플레이어가 있으면 false 반환
                    return false;
                }
            }
        }

        // 모든 플레이어가 로딩을 완료했으면 true 반환
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