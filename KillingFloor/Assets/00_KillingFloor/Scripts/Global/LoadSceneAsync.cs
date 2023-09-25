using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : MonoBehaviourPun
{
    private bool loadComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitPlayersLoadingComplete());
    }

    public void AsyncLoadScene(string name)
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
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.99f�� �ε��� 90%���� �Ϸ�Ǿ��ٴ� �ǹ�
            Debug.Log("Loading progress: " + progress * 100 + "%");

            // ������ �����ϸ� �� Ȱ��ȭ
            if (progress >= 1.0f)
            {
                if (CheckIfAllPlayersNextLoad())
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        yield return new WaitForSeconds(1.0f);
                    }

                        asyncLoad.allowSceneActivation = true; // �� Ȱ��ȭ ���
                }
            }

            // ���� �����ӱ��� ���
            yield return null;
        }
    }

    private bool CheckIfAllPlayersNextLoad()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object loadingProgressObj;
            if (player.CustomProperties.TryGetValue("LoadingProgress", out loadingProgressObj))
            {
                float loadingProgress = (float)loadingProgressObj;
                if (loadingProgress < 1.0f)
                {
                    // �ε� ���� ��Ȳ�� 1.0 �̸��� �÷��̾ ������ false ��ȯ
                    return false;
                }
            }
        }

        // ��� �÷��̾��� �ε� ���� ��Ȳ�� 1.0 �̻��̸� true ��ȯ
        return true;
    }

    private IEnumerator WaitPlayersLoadingComplete()
    {
        while (!loadComplete)
        {
            // ��� �÷��̾ �ε��� �Ϸ��ߴ��� Ȯ��
            bool allPlayersLoaded = CheckIfAllPlayersLoaded();

            if (allPlayersLoaded)
            {
                // ��� �÷��̾ �ε��� �Ϸ��ϸ� �� �ε� ����
                AsyncLoadScene("Main");
                loadComplete = true;
            }

            yield return null; // ���� �������� Ȯ��
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
}