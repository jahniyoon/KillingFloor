using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShop : MonoBehaviour
{
    public Animator animator;
    public float rotationSpeed = 45.0f; // 회전 속도 (각도/초)

    public GameObject[] Lod;

    public Material[] changeMat;

    public int colorIndex;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    public void ChangeMat()
    {
        colorIndex++;
        if(colorIndex > changeMat.Length)
        {
            colorIndex = 0;
        }
        for (int i = 0; i < Lod.Length; i++)
        {
            Lod[i].GetComponent<SkinnedMeshRenderer>().material = changeMat[colorIndex];
        }
    }

    public void EquipMat(int index)
    {
        for (int i = 0; i < Lod.Length; i++)
        {
            Lod[i].GetComponent<SkinnedMeshRenderer>().material = changeMat[index];
        }
    }
}
