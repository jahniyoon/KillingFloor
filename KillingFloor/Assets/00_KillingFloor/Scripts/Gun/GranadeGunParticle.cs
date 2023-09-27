using Photon.Pun;
using UnityEngine;
public class GranadeGunParticle : MonoBehaviourPun
{
    private GameObject user;
    private PlayerHealth playerHealth;

    private float damage = 5f;
    private float coolTime = 0.05f;
    private void OnParticleCollision(GameObject other)
    {

        GameObject IdOBj = FindTopmostParent(gameObject.transform).gameObject;

        int viewID = IdOBj.GetComponent<GranadeGun>().viewId;
        if (viewID != null)
        {
            user = PhotonView.Find(viewID).gameObject;
        }


        playerHealth = user.GetComponent<PlayerHealth>();


        Debug.Log(FindTopmostParent(other.transform));
        if (FindTopmostParent(other.transform).name.Contains("Zombie"))
        {
            if (FindTopmostParent(other.transform).gameObject.GetComponent<NormalZombie>().health > 0)
            {

                Debug.Log("들어왔다구" + other.name);
                other.transform.GetComponent<HitPoint>().Hit(60); // 좀비에게 데미지


                // 만약 좀비가 죽는다면
                if (other.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
                {
                    // 코인 먹이고
                    playerHealth.GetCoin(other.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                    // 코인값 초기화
                    other.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                    //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
                }
            }

        }
        if ("Mesh_Alfa_2".Equals(other.name)) // 보스 일경우
        {

            Debug.Log(other.name);

            other.gameObject.GetComponent<BossController>().OnDamage(20);

        }

        if ("DevilEye".Equals(other.name))
        {

            Debug.Log("메테오");

            other.gameObject.GetComponent<Meteor>().OnDamage(20);

        }
        // 보스일 경우
        if (other.transform.GetComponent<BossController>() != null)
        {
            // 보스 데미지 넣어야하는 부분
            //_hitObj.transform.GetComponent<BossController>().bossHp -= damage;
        }

        
      
       
          
        
        // 상대방으로부터 LivingEntity 타입을 가져오기 시도
        LivingEntity attackTarget = other.GetComponent<LivingEntity>();
        if (attackTarget != null)
        {                // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
            Vector3 hitPoint = other.transform.position;
            Vector3 hitNormal = transform.position - other.transform.position;

            // 공격 실행
            attackTarget.OnDamage(1, hitPoint, hitNormal);
        }
      


    }
    private Transform FindTopmostParent(Transform currentTransform)
    {
        if (currentTransform.parent == null)
        {
            // 현재 Transform이 루트이면 최상위 부모이므로 반환합니다.
            return currentTransform;
        }
        else
        {
            if(currentTransform.name.Contains("(Clone"))
            {
                return currentTransform;
            }
            else
            {
                // 부모가 있으면 부모의 부모를 재귀적으로 찾습니다.
                return FindTopmostParent(currentTransform.parent);
            }
          
        }
    }
}
