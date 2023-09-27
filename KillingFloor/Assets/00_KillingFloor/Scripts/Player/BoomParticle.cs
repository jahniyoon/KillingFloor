using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BoomParticle : MonoBehaviourPun
{
    private GameObject user;
    private PlayerHealth playerHealth;
    private void OnParticleCollision(GameObject other)
    {
        
            GameObject IdOBj = FindTopmostParent(gameObject.transform).gameObject;

            int viewID = IdOBj.GetComponent<Grenade>().viewId;
        if (viewID != null)
        {
            user = PhotonView.Find(viewID).gameObject;
            }
            playerHealth = user.GetComponent<PlayerHealth>();

         

        if (FindTopmostParent(other.transform).name.Contains("Zombie"))
        {
            if (other.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
            {
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
            // 부모가 있으면 부모의 부모를 재귀적으로 찾습니다.
            return FindTopmostParent(currentTransform.parent);
        }
    }
}
