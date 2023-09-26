using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{   public enum Type { Pistol, Rifle, Melee, Heal };


    public Transform targetObj;             // 플레이어 시점
    public Transform leftHandObj = null;    // 왼손 그랩
    public Transform rightHandObj = null;   // 오른손 그랩
    public Transform leftElbowObj = null;    // 왼손 그랩
    public Transform rightElbowObj = null;   // 오른손 그랩

    [Header("Weapon")]
    public int weaponID;
    public Type weaponType;
    public float damage;       // 데미지
    public float fireRate;     // 연사속도 : RPM으로 계산
    public float reloadRate;   // 재장전 속도 : 초로 계산
    public float ammo;         // 현재 탄창
    public float remainingAmmo;// 잔여 탄약
    public float magazineSize; // 탄창 용량
    public float maxAmmo;      //최대 총알

    public AudioClip gunAudio;    // 총 소리
    public AudioClip reloadAudio; // 장전 소리
    public AudioClip emptyAudio;  // 빈 총 소리

}
