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
        // 비동기 씬 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        asyncLoad.allowSceneActivation = false; // 씬 로딩 완료 후 자동 활성화 막기

        // 로딩이 완료될 때까지 대기
        StartCoroutine(WaitForSceneLoad(asyncLoad));
    }

    private IEnumerator WaitForSceneLoad(AsyncOperation asyncLoad)
    {
        while (!asyncLoad.isDone)
        {
            // 로딩 진행 상황을 표시하거나 추가 작업 수행 가능
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9f는 로딩이 90%까지 완료되었다는 의미

            // 조건을 만족하면 씬 활성화
            if (progress >= 1.0f)
            {
                if (CheckIfAllPlayersLoaded())
                {
                    if (PhotonNetwork.IsMasterClient) { Load(); }

                    while (!isCheck) { yield return null; }

                    asyncLoad.allowSceneActivation = true;
                }
            }

            // 다음 프레임까지 대기
            yield return null;
        }
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