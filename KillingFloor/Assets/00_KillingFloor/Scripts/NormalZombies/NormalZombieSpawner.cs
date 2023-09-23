using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombieSpawner : MonoBehaviourPun
{
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

            StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        }
        else if (GameManager.instance.wave == 2 && GameManager.instance.isCheck)
        {
            GameManager.instance.isCheck = false;
            roundPointCount = 6;

            Count();

            StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        }
        else if (GameManager.instance.wave == 3 && GameManager.instance.isCheck)
        {
            GameManager.instance.isCheck = false;
            roundPointCount = 5;

            Count();

            StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        }
        else if (GameManager.instance.wave == 4 && GameManager.instance.isCheck)
        {
            GameManager.instance.isCheck = false;
            roundPointCount = 7;

            Count();

            StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        }
        else if (GameManager.instance.wave == 5 && GameManager.instance.isCheck)
        {
            GameManager.instance.isCheck = false;
            roundPointCount = 2;

            Count();

            StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        }
    }

    private void Count()
    {
        zombieCount = GameManager.instance.wave * 1 +
                    GameManager.instance.player * 1 +
                    GameManager.instance.difficulty * 1;
    }

    private void CreateZombie()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject newObject = PhotonNetwork.Instantiate(zombiePrefab[randZombieNum].name, spawnPoint[pointCount].transform.position, Quaternion.identity);
            //ZombieTransfrom(newObject);
            
            newObject.transform.SetParent(zombieSaveList[randZombieNum]);
        }

        //GameObject newObject = Instantiate(zombiePrefab[randZombieNum], zombieSaveList[randZombieNum]);

        //newObject.transform.position = spawnPoint[pointCount].transform.position;

        Debug.Log(zombieSaveList[randZombieNum]);
        Debug.Log(randZombieNum);
    }

    //public void ZombieTransfrom(GameObject _object)
    //{
    //    photonView.RPC("MasterZombieTransform", RpcTarget.MasterClient, _object);
        
    //}

    //[PunRPC]

    //public void MasterZombieTransform(GameObject _object)
    //{
    //    photonView.RPC("SyncZombieTransfrom", RpcTarget.All, _object);
        
    //}

    //[PunRPC]
    //public void SyncZombieTransfrom(GameObject _object)
    //{
    //    _object.transform.SetParent(zombieSaveList[randZombieNum]);

        

    //}

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

                    GameManager.instance.PlusCount(1);

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

                                GameManager.instance.PlusCount(1);

                                break;
                            }
                            else { /*No Event*/ }
                        }
                        else
                        {
                            zombieSaveList[randZombieNum].GetChild(x).gameObject.SetActive(true);
                            zombieSaveList[randZombieNum].GetChild(x).position = spawnPoint[pointCount].transform.position;
                            GameManager.instance.PlusCount(1);

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

                    GameManager.instance.PlusCount(1);

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

                                GameManager.instance.PlusCount(1);

                                break;
                            }
                            else { /*No Event*/ }
                        }
                        else
                        {
                            zombieSaveList[randZombieNum].GetChild(x).gameObject.SetActive(true);
                            zombieSaveList[randZombieNum].GetChild(x).position = spawnPoint[pointCount].transform.position;

                            GameManager.instance.PlusCount(1);

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
