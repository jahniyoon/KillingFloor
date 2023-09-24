
using UnityEngine;
using UnityEngine.Rendering;

public class BFX_DemoTest : MonoBehaviour
{
    // Inspector에서 설정할 변수들
    public bool InfiniteDecal;  // 혈흔 효과를 계속해서 표시할지 여부를 결정하는 불리언 변수
    public Light DirLight;      // 주변 조명 설정
    public bool isVR = true;    // 가상 현실(VR) 모드 설정
    public GameObject BloodAttach;  // 혈흔 이펙트가 붙을 게임 오브젝트
    public GameObject[] BloodFX;    // 다양한 혈흔 이펙트 프리팹을 저장하는 배열

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

    public Vector3 direction;
    int effectIdx;
    int activeBloods;
    // 마우스 클릭 이벤트를 감지하여 혈흔 이펙트를 생성하는 업데이트 함수
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                float angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;

                if (effectIdx == BloodFX.Length) effectIdx = 0;

                // 혈흔 이펙트를 생성하고 설정
                var instance = Instantiate(BloodFX[effectIdx], hit.point, Quaternion.Euler(0, angle + 90, 0));
                effectIdx++;
                activeBloods++;
                var settings = instance.GetComponent<BFX_BloodSettings>();
                settings.LightIntensityMultiplier = DirLight.intensity;

                // 피격 지점에 가장 가까운 뼈를 찾아 혈흔 이펙트를 붙임
                var nearestBone = GetNearestObject(hit.transform.root, hit.point);
                if (nearestBone != null)
                {
                    var attachBloodInstance = Instantiate(BloodAttach);
                    var bloodT = attachBloodInstance.transform;
                    bloodT.position = hit.point;
                    bloodT.localRotation = Quaternion.identity;
                    bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
                    bloodT.LookAt(hit.point + hit.normal, direction);
                    bloodT.Rotate(90, 0, 0);
                    bloodT.transform.parent = nearestBone;
                }
            }
        }
    }

    // 두 벡터 간의 각도를 계산하는 함수
    public float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }
}
