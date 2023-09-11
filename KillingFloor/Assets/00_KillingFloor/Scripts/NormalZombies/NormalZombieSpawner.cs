using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombieSpawner : MonoBehaviour
{
    public List<Vector3> spawnPoint = new List<Vector3>();
    public List<GameObject> zombiePrefab = new List<GameObject>();
    private List<Transform> zombieSaveList = new List<Transform>();

    public GameObject zombieSave;

    public Transform zombieSaveTransform;

    private int roundPointCount;
    private int pointCount = 0;
    private int zombieCount;
    private int randZombieNum;

    private bool isCheck = true;

    private void Awake()
    {
        CreateZombieSave();
    }

    private void Start()
    {
        if (GameManager.instance.round == 1 && isCheck)
        {
            isCheck = false;
            roundPointCount = 4;

            Count();

            StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        }
    }

    private void Update()
    {
        //if (GameManager.instance.round == 1 && isCheck)
        //{
        //    isCheck = false;
        //    roundPointCount = 4;

        //    Count();

        //    StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        //}
        //else if (GameManager.instance.round == 2 && isCheck)
        //{
        //    isCheck = false;
        //    roundPointCount = 5;

        //    Count();

        //    StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        //}
        //else if (GameManager.instance.round == 3 && isCheck)
        //{
        //    isCheck = false;
        //    roundPointCount = 5;

        //    Count();

        //    StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        //}
        //else if (GameManager.instance.round == 4 && isCheck)
        //{
        //    isCheck = false;
        //    roundPointCount = 4;

        //    Count();

        //    StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        //}
        //else if (GameManager.instance.round == 5 && isCheck)
        //{
        //    isCheck = false;
        //    roundPointCount = 2;

        //    Count();

        //    StartCoroutine(SpawnZombie(zombieCount, roundPointCount));
        //}
    }

    private void Count()
    {
        zombieCount = GameManager.instance.round * 30 +
                    GameManager.instance.player * 10 +
                    GameManager.instance.difficulty * 10;
    }

    private void CreateZombie()
    {
        GameObject newObject = Instantiate(zombiePrefab[randZombieNum], zombieSaveList[randZombieNum]);

        newObject.transform.position = spawnPoint[pointCount];
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
