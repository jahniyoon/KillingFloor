using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NormalZombieSpawner : MonoBehaviourPun
{
    private Coroutine spawnCoroutine;
    public List<GameObject> spawnPoint = new List<GameObject>();
    public List<GameObject> zombiePrefab = new List<GameObject>();
    private List<Transform> zombieSaveList = new List<Transform>();

    public GameObject zombieSave;

    public Transform zombieSaveTransform;

    private int roundPointCount;
    private int pointCount = 0;
    private int zombieCount;
    private int randZombieNum;

    private void Awake()
    {
        CreateZombieSave();
    }

    private void Update()
    {
        if (GameManager.instance.wave == 1 && GameManager.instance.isCheck)
        {
            GameManager.instance.isCheck = false;
            roundPointCount = 4;

            Count();
            if (spawnCoroutine != null)
            {
                return;
            }
            else
            {
                spawnCoroutine = StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
            }
        }
        else if (GameManager.instance.wave == 2 && GameManager.instance.isCheck)
        {
            GameManager.instance.isCheck = false;
            roundPointCount = 6;

            Count();

            spawnCoroutine = StartCoroutine(SpawnZombie(zombieCount, roundPointCount));

        }
        else if (GameManager.instance.wave == 3 && GameManager.instance.isCheck)
        {
            GameManager.instance.isCheck = false;
            roundPointCount = 5;

            Count();

            spawnCoroutine = StartCoroutine(SpawnZombie(zombieCount, roundPointCount));

        }
        else if (GameManager.instance.wave == 4 && GameManager.instance.isCheck)
        {
            GameManager.instance.isCheck = false;
            roundPointCount = 7;

            Count();
            //영상 제작용
            zombieCount = 5;
            //영상 제작용
            spawnCoroutine = StartCoroutine(SpawnZombie(zombieCount, roundPointCount));

        }
        else if (GameManager.instance.wave == 5 && GameManager.instance.isCheck)
        {
            GameManager.instance.isCheck = false;
            roundPointCount = 2;

            Count();

            spawnCoroutine = StartCoroutine(SpawnZombie(zombieCount, roundPointCount));

        }
    }

    private void Count()
    {
        zombieCount = GameManager.instance.wave * 20 +
                    GameManager.instance.player * 20 +
                    GameManager.instance.difficulty * 10;
    }
    private GameObject newObject;
    private void CreateZombie()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            newObject = PhotonNetwork.Instantiate(zombiePrefab[randZombieNum].name, spawnPoint[pointCount].transform.position, Quaternion.identity);
            newObject.GetComponent<NormalZombie>().ZombieSetting();
            int viewId = newObject.GetComponent<PhotonView>().ViewID;

            if (viewId != null)
            {
                ZombieParent(viewId, spawnPoint[pointCount].transform.position);
            }
        }
    }

    private void ZombieParent(int _newObject, Vector3 _newObjectPos)
    {
        photonView.RPC("MasterParent", RpcTarget.MasterClient, _newObject, _newObjectPos);
    }

    [PunRPC]
    public void MasterParent(int _newObject, Vector3 _newObjectPos)
    {
        photonView.RPC("SyncParent", RpcTarget.All, _newObject, _newObjectPos);
    }

    [PunRPC]
    public void SyncParent(int _newObject, Vector3 _newObjectPos)
    {
        GameObject saveObj = PhotonView.Find(_newObject)?.gameObject;

        for (int i = 0; i < zombieSaveList.Count; i++)
        {
            if (zombieSaveList[i].name + "(Clone)" == saveObj.name)
            {
                saveObj.transform.SetParent(zombieSaveList[i]);
            }
        }

        saveObj.transform.position = _newObjectPos;
    }

    private void CreateZombieSave()
    {
        for (int i = 0; i < zombiePrefab.Count; i++)
        {
            GameObject newTransform = Instantiate(zombieSave, zombieSaveTransform);
            newTransform.name = zombiePrefab[i].name;
            zombieSaveList.Add(newTransform.transform);
        }
    }

    public IEnumerator SpawnZombie(int _zombieCount, int _roundPointCount)
    {
        for (int i = 0; i < _roundPointCount; i++)
        {
            for (int j = 0; j < _zombieCount / _roundPointCount * 0.8f; j++)
            {
                randZombieNum = Random.Range(0, 4);

                if (zombieSaveList[randZombieNum].childCount == 0)
                {
                    CreateZombie();

                    continue;
                }
                else
                {
                    for (int x = 0; x < zombieSaveList[randZombieNum].childCount; x++)
                    {
                        if (zombieSaveList[randZombieNum].GetChild(x).gameObject.activeSelf)
                        {
                            if (x == zombieSaveList[randZombieNum].childCount - 1)
                            {
                                CreateZombie();

                                break;
                            }
                        }
                        else
                        {
                            zombieSaveList[randZombieNum].GetChild(x).gameObject.SetActive(true);
                            zombieSaveList[randZombieNum].GetChild(x).gameObject.GetComponent<NormalZombie>().ZombieSetting();
                            zombieSaveList[randZombieNum].GetChild(x).position = spawnPoint[pointCount].transform.position;

                            break;
                        }
                    }
                }

                yield return null;
            }
            for (int j = 0; j < _zombieCount / _roundPointCount * 0.2f; j++)
            {
                randZombieNum = Random.Range(4, 9);

                if (zombieSaveList[randZombieNum].childCount == 0)
                {
                    CreateZombie();

                    continue;
                }
                else
                {
                    for (int x = 0; x < zombieSaveList[randZombieNum].childCount; x++)
                    {
                        if (zombieSaveList[randZombieNum].GetChild(x).gameObject.activeSelf)
                        {
                            if (x == zombieSaveList[randZombieNum].childCount - 1)
                            {
                                CreateZombie();

                                break;
                            }
                            else { /*No Event*/ }
                        }
                        else
                        {
                            zombieSaveList[randZombieNum].GetChild(x).gameObject.SetActive(true);
                            zombieSaveList[randZombieNum].GetChild(x).gameObject.GetComponent<NormalZombie>().ZombieSetting();
                            zombieSaveList[randZombieNum].GetChild(x).position = spawnPoint[pointCount].transform.position;

                            break;
                        }
                    }
                }

                yield return null;
            }

            pointCount += 1;
        }
    }
}
