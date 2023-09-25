using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BloodEffect : MonoBehaviour
{
    public bool InfiniteDecal;      // 혈흔 효과를 계속 표시할지 여부
    public Light DirLight;          // 주변 조명 설정
    public GameObject BloodAttach;  // 혈흔 효과가 붙을 게임 오브젝트
    public GameObject[] BloodFX;    // 혈흔 이펙트 프리팹

    public Vector3 direction;
    int effectIdx;
    int activeBloods;

    // 레이캐스트를 통해 얻은 히트 포인트에서 가장 가까운 뼈를 찾는 함수
    Transform GetNearestObject(Transform hit, Vector3 hitPos)
    {
        var closestPos = 100f;
        Transform closestBone = null;
        var childs = hit.GetComponentsInChildren<Transform>();

        foreach (var child in childs)
        {
            var dist = Vector3.Distance(child.position, hitPos);
            if (dist < closestPos)
            {
                closestPos = dist;
                closestBone = child;
            }
        }

        var distRoot = Vector3.Distance(hit.position, hitPos);
        if (distRoot < closestPos)
        {
            closestPos = distRoot;
            closestBone = hit;
        }
        return closestBone;
    }

    public void Start()
    {
        DirLight = GameObject.Find("Directional Light").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnBloodEffect(Vector3 hitPoint, float angle, Vector3 hitNormal, int _viewID)
    {

        if (effectIdx == BloodFX.Length) effectIdx = 0;

        // 혈흔 이펙트를 생성하고 설정
        var instance = Instantiate(BloodFX[effectIdx], hitPoint, Quaternion.Euler(0, angle + 90, 0));
        effectIdx++;
        activeBloods++;
        var settings = instance.GetComponent<BFX_BloodSettings>();
        settings.LightIntensityMultiplier = DirLight.intensity;
        Destroy(instance, 10f);

        PhotonView hitObj = PhotonView.Find(_viewID); // viewID는 찾으려는 PhotonView의 ID입니다.
        if (hitObj != null)
        {
            Debug.Log(hitObj + "피를 그린다");
            GameObject targetObject = hitObj.gameObject;
            // 이제 targetObject는 해당 PhotonView를 가진 게임 오브젝트입니다.


            //// 피격 지점에 가장 가까운 뼈를 찾아 혈흔 이펙트를 붙임
            //var nearestBone = GetNearestObject(hit.transform.root, hitPoint);

            var attachBloodInstance = Instantiate(BloodAttach);
            var bloodT = attachBloodInstance.transform;
            bloodT.position = hitPoint;
            bloodT.localRotation = Quaternion.identity;
            bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
            bloodT.LookAt(hitPoint + hitNormal, direction);
            bloodT.Rotate(90, 0, 0);
            bloodT.transform.parent = targetObject.transform;
            Destroy(attachBloodInstance,10f);
        }
    }

    // 두 벡터 간의 각도를 계산하는 함수
    public float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }

}
