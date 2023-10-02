using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerShooter : MonoBehaviourPun
{
    public enum State
    {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }
    //ssm 유탄 발사 로직 //

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private GameObject bulletPoint;
    [SerializeField]
    private float bulletSpeed = 600;
    [SerializeField]
    private List<GameObject> bulletlist;

    //ssm 유탄 끝
    public State state { get; private set; }
    public enum Type { Pistol, Rifle, Melee, Heal };
    private PlayerInputs input;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private CameraSetup cameraSet;
    private BloodEffect bloodFX;
    protected Animator animator;
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 14);    // 데미지 받을 좀비의 레이어 마스크


    public Transform aimTarget; // 플레이어가 보는 방향
    public Transform targetObj;                // 플레이어 시점
    public Transform weaponPosition = null;    // 무기 위치 기준점
    public Transform rightHandPosition; // 오른손 위치

    // 병과
    public enum WeaponClass { Commando, Demolitionist };    // 병과 추가될경우 여기에 추가
    public WeaponClass weaponClass;                         // 현재 병과 상태

    [Header("Weapon Info")]
    public Weapon equipedWeapon;
    public Weapon[] weaponList;
    [Range(1, 5)]
    public int weaponSlot = 1;
    public float damage;        // 총기 데미지
    public float range = 100f;  // 사거리
    public float reloadRate;    // 재장전 속도
    public float fireRate;      // 사격 속도
    public float lastFireTime;  // 마지막 사격시간
    public float healCoolDown = 15f;  // 힐 쿨다운

    [Header("Effects")]

    public ParticleSystem muzzleFlashEffect; // 총구 화염효과
    public ParticleSystem shellEjectEffect;  // 탄피 배출 효과
    public AudioSource gunAudioPlayer;       // 총 소리 재생기

    public Transform fireTransform;          // 총알이 발사될 위치
    public ParticleSystem bulletHole;            // 총알이 맞는 곳에 생성되는 파티클
    public GameObject bloodParticle;            // 총알이 맞는 곳에 생성되는 파티클
    public LineRenderer bulletLineRenderer;  // 총알 궤적을 그리기 위한 렌더러
    public ParticleSystem fireParticle;
    public bool isParticleTrigger;          // 파티클 생성여부 트리거
    public bool isBloodTrigger;             // 혈흔효과 여부 트리거

    [Header("Grenade")]
    public int grenade;         // 수류탄 개수
    public bool isGrenade;      // 수류탄 상태 체크 (1인칭 애니메이션)
    public GameObject grenadePrefab;  // 수류탄 프리팹 (Resources 폴더)
    public Transform throwPosition;   // 던지는 포지션
    public float grenadePower;        // 수류탄 던지는 힘


    [Header("TPS Weapon")]
    public Weapon tpsPistol;    // 가져올 권총 무기 정보
    public Weapon tpsRifle;     // 가져올 라이플 무기 정보
    public Weapon tpsMelee;     // 가져올 근접 무기 정보
    public Weapon tpsHeal;     // 가져올 근접 무기 정보
    public Weapon tpsSMG;
    public Weapon tpsSCAR;

    [Header("FPS Weapon")]
    public Transform fpsPosition;
    public Transform fpsPistol;
    public Transform fpsSMG;
    public Transform fpsRifle;
    public Transform fpsSCAR;
    public Transform fpsMelee;
    public Transform fpsHeal;
    public Transform fpsGrenade;

    public bool isSMG;
    public bool isSCAR;

    [Header("Animator IK")]
    public Animator handAnimator;
    public Transform rightHandObj = null;   // 오른손
    public Transform leftHandObj = null;    // 왼손
    public Transform rightElbowObj = null;   // 오른손 그랩
    public Transform leftElbowObj = null;    // 왼손 그랩
    [Range(0, 1)]
    public float handIKAmount = 1;
    [Range(0, 1)]
    public float elbowIKAmount = 1;
    [Range(0, 1)]
    public float animationIKAmount = 0.5f; // 애니메이션 중 IK 기본값
    public bool ikActive = false;

    private void Awake()
    {
        switch (GameManager.instance.playerClass)
        {
            case "Commando":
                weaponClass = WeaponClass.Commando;
                break;
            case "Demolitionist":
                weaponClass = WeaponClass.Demolitionist;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        bulletlist = new List<GameObject>();


        for (int i = 0; i < 7; i++)
        {

            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, bulletPoint.transform.forward, transform.rotation);

            bulletlist.Add(bullet);
            bulletlist[i].GetComponent<GranadeGun>()?.setViewId(photonView.ViewID);
            bulletlist[i].SetActive(false);
        }







        input = GetComponent<PlayerInputs>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        cameraSet = GetComponent<CameraSetup>();
        animator = GetComponent<Animator>();
        bloodFX = GetComponent<BloodEffect>();

        // ========================= 무기 가져오는 부분 =========================//
        // TPS 무기 가져오기
        tpsPistol = weaponPosition.GetChild(0).GetChild(0).GetComponent<Weapon>();
        tpsMelee = weaponPosition.GetChild(2).GetComponent<Weapon>();
        tpsHeal = weaponPosition.GetChild(3).GetComponent<Weapon>();

        


        // FPS 무기 가져오기
        fpsPosition = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>(); // FPS Body 게임오브젝트
        fpsPistol = fpsPosition.transform.GetChild(0).GetChild(0).GetComponent<Transform>();   // Slot1 의 무기 가져오기
        fpsMelee = fpsPosition.transform.GetChild(2).GetComponent<Transform>();
        fpsHeal = fpsPosition.transform.GetChild(3).GetComponent<Transform>();
        fpsGrenade = fpsPosition.transform.GetChild(4).GetComponent<Transform>();

        // 병과에 따라 가져올 무기가 달라져야하는 경우
        switch (weaponClass)
        {

            // 코만도면 Slot2의 첫번째 무기
            case WeaponClass.Commando:
                fpsRifle = fpsPosition.transform.GetChild(1).GetChild(0).GetComponent<Transform>();
                tpsRifle = weaponPosition.GetChild(1).GetChild(0).GetComponent<Weapon>();
                fpsPosition.transform.GetChild(1).GetChild(1).GetComponent<Transform>().gameObject.SetActive(false);
                weaponPosition.GetChild(1).GetChild(1).GetComponent<Weapon>().gameObject.SetActive(false);

                break;
            // 데몰리스트면 Slot2의 두번째 무기
            case WeaponClass.Demolitionist:

                fpsRifle = fpsPosition.transform.GetChild(1).GetChild(1).GetComponent<Transform>();
                tpsRifle = weaponPosition.GetChild(1).GetChild(1).GetComponent<Weapon>();
                fpsPosition.transform.GetChild(1).GetChild(0).GetComponent<Transform>().gameObject.SetActive(false);
                weaponPosition.GetChild(1).GetChild(0).GetComponent<Weapon>().gameObject.SetActive(false);
                break;

        }


        tpsRifle.gameObject.SetActive(false);    // 미리 꺼두기
        tpsMelee.gameObject.SetActive(false);    // 미리 꺼두기
        tpsHeal.gameObject.SetActive(false);    // 미리 꺼두기

        fpsRifle.gameObject.SetActive(false);
        fpsMelee.gameObject.SetActive(false);
        fpsHeal.gameObject.SetActive(false);
        fpsGrenade.gameObject.SetActive(false);

        // 구매가능한 무기들은 가져와서 미리 꺼두기
        fpsSMG = fpsPosition.transform.GetChild(0).GetChild(1).GetComponent<Transform>();   // Slot1 의 무기 가져오기
        fpsSCAR = fpsPosition.transform.GetChild(1).GetChild(2).GetComponent<Transform>();
        fpsSMG.gameObject.SetActive(false);
        fpsSCAR.gameObject.SetActive(false);

        tpsSMG = weaponPosition.GetChild(0).GetChild(1).GetComponent<Weapon>();
        tpsSCAR = weaponPosition.GetChild(1).GetChild(2).GetComponent<Weapon>(); // 스카 가져오기
        tpsSMG.gameObject.SetActive(false);
        tpsSCAR.gameObject.SetActive(false);

        SetWeapon(tpsPistol, fpsPistol); // 무기 장착
        animator.SetBool("isWeaponPistol", true);
        animator.SetBool("isWeaponRifle", false);


        lastFireTime = 0;       // 시간 초기화

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) { return; } // 로컬 플레이어가 아닌 경우 입력을 받지 않는다.

        // 입력 가능여부 확인
        if (GameManager.instance != null && !GameManager.instance.inputLock)
        {
            HandSet();
            Aim();
            Fire();
            Reload();
            WeaponInput();
            Weapons();
            Melee();
            Heal();
        }
    }

    // 주무기 보조무기 사격 입력
    void Fire()
    {

        if (input.shoot && weaponSlot < 3 && weaponClass == WeaponClass.Commando ||
            input.shoot && weaponSlot == 1 && weaponClass == WeaponClass.Demolitionist)
        {
            // 현재 상태가 발사 가능한 상태
            // && 마지막 총 발사 시점에서 timeBetFire 이상의 시간이 지남
            if (state == State.Ready && Time.time >= lastFireTime + fireRate && !input.dash && 0 < equipedWeapon.ammo)
            {
                // 마지막 총 발사 시점을 갱신
                lastFireTime = Time.time;
                // 실제 발사 처리 실행
                Shot();
             
            }
            // 남은 총알이 있을 때 발사하면 재장전 실행
            else if (state == State.Empty && 0 < equipedWeapon.remainingAmmo && !input.dash)
            {
                input.shoot = false;
                input.reload = true; // 재장전 버튼 눌러주기
            }
            // 남은 총알도 없을 때
            else if (equipedWeapon.ammo == 0 && equipedWeapon.remainingAmmo == 0 && !input.dash)
            {
                // ToDo : 틱 사운드 플레이되도록 하기 (총알 없음)
                gunAudioPlayer.clip = equipedWeapon.emptyAudio;
                gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // 총소리 재생

                input.shoot = false;
            }
        }
        if (input.shoot && weaponSlot < 3 && weaponClass == WeaponClass.Demolitionist)
        {
            if (state == State.Ready && Time.time >= lastFireTime + fireRate && 0 < equipedWeapon.ammo)
            {
                lastFireTime = Time.time;
                if (weaponSlot == 2)
                {
                    photonView.RPC("GranadeActMaster", RpcTarget.MasterClient);
                    equipedWeapon.ammo -= 1;
                    PlayerUIManager.instance.SetAmmo(equipedWeapon.ammo);           // 현재 탄 UI 세팅
                }
            }
            // 데몰리스트 발사 입력 후 할것들
        }
        PlayerUIManager.instance.SetCoin(playerHealth.coin);

    }
    [PunRPC]
    void GranadeActMaster()
    {
        photonView.RPC("GranadeActAll", RpcTarget.All);
    }
    [PunRPC]
    void GranadeActAll()
    {
        for (int i = 0; i < 7; i++)
        {
            if (!bulletlist[i].activeSelf)
            {


                bulletlist[i].transform.position = bulletPoint.transform.position;
                bulletlist[i].transform.rotation = bulletPoint.transform.rotation;

                bulletlist[i].GetComponent<Rigidbody>().velocity = Vector3.zero; // 이전 속도 초기화
                bulletlist[i].SetActive(true);
                bulletlist[i].GetComponent<Rigidbody>().AddForce(bulletPoint.transform.forward * bulletSpeed * 2f);
                handAnimator.SetTrigger("isFire");
                animator.SetTrigger("isFire");
               

                if (equipedWeapon.ammo <= 0)
                {
                    // 탄창에 남은 탄약이 없다면, 총의 현재 상태를 Empty으로 갱신
                    state = State.Empty;
                    input.shoot = false;
                }
                break;
            }
        }
    }
    void Shot()
    {

        Vector3 cameraForward = cameraSet.followCam.transform.forward;
        Vector3 cameraPosition = cameraSet.followCam.transform.position;
        // 카메라는 각자 가지고있으므로 카메라값도 함께 전달
        //Debug.Log(photonView.ViewID + "마스터에게 사격 요청");


        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient, cameraForward, cameraPosition);



        // 애니메이션 작동 
        handAnimator.SetTrigger("isFire");
        animator.SetTrigger("isFire");
        PlayerFireCameraShake.Invoke();
        equipedWeapon.ammo -= 1;
        PlayerUIManager.instance.SetAmmo(equipedWeapon.ammo);           // 현재 탄 UI 세팅

        if (equipedWeapon.ammo <= 0)
        {
            // 탄창에 남은 탄약이 없다면, 총의 현재 상태를 Empty으로 갱신
            state = State.Empty;
            input.shoot = false;
        }
        // 총을 쏠 때 마다 살짝씩 올라가게 하기
        input.LookInput(new Vector2(0, -0.7f));
        Invoke("ResetLookInput", 0.1f);
        //input.look = Vector2.Lerp(new Vector2(0, input.look.y - 5), Vector2.zero, 1f);

    }
    // 총기 반동 초기화
    public void ResetLookInput()
    {
        input.LookInput(new Vector2(0,0));

    }

    // 마스터가 실행하는 실제 발사 처리
    [PunRPC]
    private void ShotProcessOnServer(Vector3 _cameraForward, Vector3 _cameraPosition)
    {

        // ToDo : 히트를 밖에서 수정해줘야함. 히트포인트를 받아와야지 데미지 효과 실행 가능
        // 레이캐스트에 의한 충돌 정보를 저장하는 컨테이터
        RaycastHit hit;
        Vector3 hitPoint = _cameraForward * range;
        GameObject hitObj = null;

        // 혈흔효과에 사용할 각도
        float angle = 0;
        // 혈흔효과가 덮힐 오브젝트
        Transform hitTransformRoot = null;

        // 잠깐 IK 풀어주기
        handIKAmount = animationIKAmount;
        elbowIKAmount = animationIKAmount;
        StartCoroutine(ShootCoroutine());

        // 만약 좀비류에 닿으면 데미지
        if (Physics.Raycast(_cameraPosition, _cameraForward, out hit, range, layerMask))
        {
            // 혈흔 효과가 덮힐 오브젝트 정해주기
            hitTransformRoot = hit.transform.root;
            // 혈흔효과에 사용할 각도
            angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;
            hitObj = hit.transform.gameObject;
            hitPoint = hit.point;
            isBloodTrigger = true;  // 혈흔효과 트리거
        }

        // 만약 뭔가에 닿으면 그곳을 히트포인트로
        else if (Physics.Raycast(_cameraPosition, _cameraForward, out hit, range))
        {
            hitPoint = hit.point;
            isParticleTrigger = true;
        }

        // 발사처리를 마스터에게 위임
        if (hitObj != null)
        {
            // 부딧친 오브젝트가 있다면 모두에게 데미지 실행
            Damage(hitObj);
        }


        Vector3 hitNormal = hit.normal;
        int viewID = 999999;    // null 값을 알기위한 임의의 숫자

        //Debug.Log("좀비류에 닿았나? " + hitTransformRoot);

        if (hitTransformRoot != null)
        {
            var nearestBone = GetNearestObject(hitTransformRoot, hitPoint);
            if (nearestBone)
                //Debug.Log("뼈가 있나? : " + nearestBone);

                if (nearestBone.gameObject.GetPhotonView() == null)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        //Debug.Log("포톤뷰가 없다. 상위로 전환 : " + nearestBone);
                        nearestBone = nearestBone.parent;
                        if (nearestBone.gameObject.GetPhotonView() != null)
                        {
                            break;
                        }
                    }
                }
            if (nearestBone != null && nearestBone.gameObject.GetPhotonView() != null)
            {

                viewID = nearestBone.gameObject.GetPhotonView().ViewID;
                //Debug.Log("ViewID 부여 : " + viewID);
            }
        }
        // 이펙트 재생 코루틴을 랩핑
        photonView.RPC("ShotEffectProcessOnClients", RpcTarget.All, hitPoint, hitNormal, angle, viewID, isBloodTrigger, isParticleTrigger);

    }

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

    // 이펙트 재생 코루틴
    [PunRPC]
    private void ShotEffectProcessOnClients(Vector3 hitPosition, Vector3 _hitNormal, float _angle, int _viewID, bool _blood, bool _bullet)
    {
        StartCoroutine(ShotEffect(hitPosition, _hitNormal, _angle, _viewID, _blood, _bullet));
    }
    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다.
    private IEnumerator ShotEffect(Vector3 _hitPosition, Vector3 _hitNormal, float _angle, int _viewID, bool _blood, bool _bullet)
    {
        //Debug.Log("모두 이펙트를 실행한다.");

        if (_bullet)
        {
            //Debug.Log("총알자국 파티클 생성");

            // 총알 자국 파티클 생성
            ParticleSystem _bulletHoleParticle = bulletHole;
            _bulletHoleParticle.transform.position = _hitPosition;
            bulletHole.Play();
            isParticleTrigger = false;
        }
        if (_blood)
        {
            //Debug.Log("블러드 생성");
            bloodFX.OnBloodEffect(_hitPosition, _angle, _hitNormal, _viewID);
            isBloodTrigger = false;
        }
        fireParticle.Play();    // 파티클 재생
        gunAudioPlayer.clip = equipedWeapon.gunAudio;
        gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // 총소리 재생

        // 선의 시작점은 총구의 위치
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        // 선의 끝점은 입력으로 들어온 충돌 위치
        bulletLineRenderer.SetPosition(1, _hitPosition);
        // 라인 렌더러를 활성화하여 총알 궤적을 그린다
        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.01f);
        // 라인렌더러 비활성화
        bulletLineRenderer.enabled = false;
    }

    // 플레이어 조준키 입력
    void Aim()
    {
        // 대시중일 때는 조준 애니메이션 False
        if (input.dash)
        {
            if (weaponSlot <= 2)
            {
                handAnimator.SetBool("isAim", false);
            }
            return;
        }

        // 주무기, 보조무기 재장전 중이거나 대시중이 아닐 때 조준
        if (weaponSlot <= 2 && state == State.Ready && !input.dash)
        {
            handAnimator.SetBool("isAim", input.aim);
        }

        // 밀리 무기상태이면 강공격 실행
        if (weaponSlot == 3 && state == State.Ready && input.aim)
        {
            state = State.Reloading;
            handAnimator.SetTrigger("isAim");
            gunAudioPlayer.clip = equipedWeapon.reloadAudio;
            gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // 근접공격 소리 재생
            StartCoroutine(WeaponDelay(reloadRate * 1.8f));
            input.aim = false;
        }
    }
    // 사격 딜레이를 주기위한 코루틴
    IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(fireRate); // fireRate 는 RPM
        // 단발 설정
        if (weaponSlot == 1 && !isSMG)
        {
            input.shoot = false;
        }
        handIKAmount = 1f;
        elbowIKAmount = 1f;
    }

    // 밀리 또는 힐의 딜레이를 주기위한 코루틴
    IEnumerator WeaponDelay(float _reloadRate)
    {
        yield return new WaitForSeconds(_reloadRate);
        state = State.Ready;
    }

    void Melee()
    {
        // 근접공격
        if (input.shoot && weaponSlot == 3 && state == State.Ready && !input.dash)
        {
            state = State.Reloading;
            handAnimator.SetTrigger("isFire");
            gunAudioPlayer.clip = equipedWeapon.gunAudio;
            gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // 근접공격 소리 재생

            StartCoroutine(WeaponDelay(reloadRate));
            input.shoot = false;
        }
    }
    [PunRPC]
    public void MeleeBlood(Vector3 _hitPosition, int _viewID)
    {
        Debug.Log("피 효과 요청 " + _viewID);
        bloodFX.OnBloodEffect(_hitPosition, 0, Vector3.zero, _viewID);
    }
    void Heal()
    {
        // 힐 클릭
        if (input.shoot && weaponSlot == 4 && state == State.Ready && 15 <= healCoolDown && playerHealth.health != 100 && !input.dash)
        {
            state = State.Reloading;
            handAnimator.SetTrigger("isFire");
            healCoolDown = -0.1f;
            StartCoroutine(WeaponDelay(reloadRate));
            float heal = damage;
            if (heal + playerHealth.health >= 100)
            { heal -= ((heal + playerHealth.health) - 100); }
            photonView.RPC("HealProcessOnServer", RpcTarget.All, heal);
            //PlayerUIManager.instance.SetHP(playerHealth.health);
            input.shoot = false;
        }

        // 쿨다운 업데이트
        if (healCoolDown <= 15)
        {
            healCoolDown += Time.deltaTime;
            PlayerUIManager.instance.SetHeal(healCoolDown);
        }
    }
    // 힐 동기화
    [PunRPC]
    void HealProcessOnServer(float _heal)
    {
        playerHealth.RestoreHealth(_heal);
    }


    void Damage(GameObject _hitObj)
    {
        Debug.Log(_hitObj.name);

        if (_hitObj.transform.GetComponent<HitPoint>() == null && _hitObj.transform.GetComponent<PlayerDamage>() != null)
        {
            playerHealth.GetCoin(100);  // Debug 디버그용 재화 획득
            playerHealth.ExpUp(8);
            _hitObj.transform.GetComponent<PlayerDamage>().OnDamage(); // RPC 확인 디버그용
            return;
        }


        if (!"Mesh_Alfa_2".Equals(_hitObj.transform.name) && !"DevilEye".Equals(_hitObj.transform.name))//보스 가 아닐경우 
        {
            ////////////////////////////////////////////////좀비////////////////////


            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
            {
                _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // 좀비에게 데미지

                // 만약 좀비가 죽는다면
                if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
                {
                    // 코인 먹이고
                    playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);
                    playerHealth.ExpUp(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                    // 코인값 초기화
                    _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                    //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
                }
            }

            ////////////////////////////////////////////////////////////////////
        }

        if ("Mesh_Alfa_2".Equals(_hitObj.name)) // 보스 일경우
        {


            if (9 == _hitObj.transform.gameObject.layer)
            {

                _hitObj.gameObject.GetComponent<BossController>().OnDamage(damage);
            }
            else if (11 == _hitObj.transform.gameObject.layer)
            {

                _hitObj.gameObject.GetComponent<BossController>().OnDamage(damage * 0.5f);
            }
        }

        if ("DevilEye".Equals(_hitObj.name))
        {

            Debug.Log("메테오");

            _hitObj.gameObject.GetComponent<Meteor>().OnDamage(damage);

        }
        // 보스일 경우
        if (_hitObj.transform.GetComponent<BossController>() != null)
        {
            // 보스 데미지 넣어야하는 부분
            //_hitObj.transform.GetComponent<BossController>().bossHp -= damage;
        }

    }

    // 장전
    public void Reload()
    {
        // 주무기, 보조무기일 때 장전 버튼을 누른다면
        if (input.reload && weaponSlot < 3)
        {
            if (Reloading())
            {
                // 애니메이션 작동 후 잠깐 IK 풀어주기
                handAnimator.SetTrigger("isReload");
                animator.SetTrigger("isReload");
                handIKAmount = animationIKAmount;
                elbowIKAmount = animationIKAmount;
                gunAudioPlayer.clip = equipedWeapon.reloadAudio;
                gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // 재장전 소리 재생

                input.shoot = false;
                input.dash = false;
            }
            input.reload = false;
        }
    }
    private bool Reloading()
    {
        // 잔여 탄이 0보다 많고, 탄이 꽉차있지 않고, 장전 가능할 때 장전
        if (state == State.Reloading || equipedWeapon.remainingAmmo <= 0 || equipedWeapon.ammo == equipedWeapon.magazineSize)
        {
            return false;
        }
        StartCoroutine(ReloadCoroutine());
        return true;
    }
    // 장전 코루틴
    IEnumerator ReloadCoroutine()
    {
        state = State.Reloading;

        yield return new WaitForSeconds(reloadRate);

        float currentAmmo = equipedWeapon.ammo;
        float remainingAmmo = equipedWeapon.remainingAmmo - (equipedWeapon.magazineSize - currentAmmo);

        equipedWeapon.ammo = Mathf.Min(equipedWeapon.magazineSize, equipedWeapon.ammo + equipedWeapon.remainingAmmo);   // 현재 탄 세팅
        equipedWeapon.remainingAmmo = Mathf.Max(0, remainingAmmo);                                                      // 남은 탄 세팅
        PlayerUIManager.instance.SetAmmo(equipedWeapon.ammo);           // 현재 탄 UI 세팅
        PlayerUIManager.instance.SetRemainingAmmo(equipedWeapon.remainingAmmo);           // 현재 탄 UI 세팅

        handIKAmount = 1f;
        elbowIKAmount = 1f;
        state = State.Ready;
    }

    // 화면에 보여지는 무기를 변경하는 메서드
    public void Weapons()
    {
        // 재장전 상태 아닐때만 변경 가능
        if (state != State.Reloading && !input.dash)
        {
            if (input.weaponSlot1)
            {
                if (weaponSlot != 1 && !isGrenade)
                {
                    weaponSlot = 1;
                    tpsRifle.gameObject.SetActive(false);
                    tpsPistol.gameObject.SetActive(true);
                    fpsRifle.gameObject.SetActive(false);
                    fpsPistol.gameObject.SetActive(true);
                    fpsMelee.gameObject.SetActive(false);
                    fpsHeal.gameObject.SetActive(false);
                    fpsGrenade.gameObject.SetActive(false);
                    SetWeapon(tpsPistol, fpsPistol); // 무기 장착
                    photonView.RPC("ServerProcessWeapon", RpcTarget.Others, 0);

                    animator.SetBool("isWeaponPistol", true);
                    animator.SetBool("isWeaponRifle", false);
                }
                input.weaponSlot1 = false;
            }
            if (input.weaponSlot2)
            {
                if (weaponSlot != 2 && !isGrenade)
                {
                    weaponSlot = 2;
                    tpsPistol.gameObject.SetActive(false);
                    tpsRifle.gameObject.SetActive(true);
                    fpsPistol.gameObject.SetActive(false);
                    fpsRifle.gameObject.SetActive(true);
                    fpsMelee.gameObject.SetActive(false);
                    fpsHeal.gameObject.SetActive(false);
                    fpsGrenade.gameObject.SetActive(false);
                    SetWeapon(tpsRifle, fpsRifle); // 무기 장착
                    photonView.RPC("ServerProcessWeapon", RpcTarget.Others, 1);

                    animator.SetBool("isWeaponPistol", false);
                    animator.SetBool("isWeaponRifle", true);
                }
                input.weaponSlot2 = false;
            }
            if (input.weaponSlot3)
            {
                if (weaponSlot != 3 && !isGrenade)
                {
                    weaponSlot = 3;
                    tpsPistol.gameObject.SetActive(false);
                    tpsRifle.gameObject.SetActive(false);
                    fpsPistol.gameObject.SetActive(false);
                    fpsRifle.gameObject.SetActive(false);
                    fpsMelee.gameObject.SetActive(true);
                    fpsHeal.gameObject.SetActive(false);
                    fpsGrenade.gameObject.SetActive(false);
                    SetWeapon(tpsMelee, fpsMelee); // 무기 장착

                }
                input.weaponSlot3 = false;
            }
            if (input.weaponSlot4)
            {
                if (weaponSlot != 4 && !isGrenade)
                {
                    weaponSlot = 4;
                    tpsPistol.gameObject.SetActive(false);
                    tpsRifle.gameObject.SetActive(false);
                    fpsPistol.gameObject.SetActive(false);
                    fpsRifle.gameObject.SetActive(false);
                    fpsMelee.gameObject.SetActive(false);
                    fpsHeal.gameObject.SetActive(true);
                    fpsGrenade.gameObject.SetActive(false);
                    SetWeapon(tpsHeal, fpsHeal); // 무기 장착

                }
                input.weaponSlot4 = false;
            }
            if (input.grenade && !isGrenade && 0 < grenade)
            {
                isGrenade = true;
                tpsPistol.gameObject.SetActive(false);
                tpsRifle.gameObject.SetActive(false);
                fpsPistol.gameObject.SetActive(false);
                fpsRifle.gameObject.SetActive(false);
                fpsMelee.gameObject.SetActive(false);
                fpsHeal.gameObject.SetActive(false);
                fpsGrenade.gameObject.SetActive(true);
                state = State.Reloading;
                StartCoroutine(Grenade());
                input.grenade = false;
            }
        }
    }

    [PunRPC]
    public void ServerProcessWeapon(int index)
    {
        Weapon _tpsWeapon = weaponList[index].GetComponent<Weapon>();
        equipedWeapon.gameObject.SetActive(false);
        _tpsWeapon.gameObject.SetActive(true);
        Debug.Log(index + "번째 무기 교체 요청 :" + _tpsWeapon);

        equipedWeapon = _tpsWeapon;
        rightHandObj = equipedWeapon.rightHandObj.transform;     // 권총의 오른손 그랩
        leftHandObj = equipedWeapon.leftHandObj.transform;       // 권총의 왼손 그랩
        rightElbowObj = equipedWeapon.rightElbowObj.transform;   // 권총의 오른팔꿈치
        leftElbowObj = equipedWeapon.leftElbowObj.transform;     // 권총의 왼팔꿈치

        // 무기 장착 및 무기 정보 세팅
        switch ((Type)equipedWeapon.weaponType)
        {
            case Type.Pistol:
                animator.SetBool("isWeaponPistol", true);
                animator.SetBool("isWeaponRifle", false);
                equipedWeapon.weaponType = Weapon.Type.Pistol;
                break;
            case Type.Rifle:
                animator.SetBool("isWeaponRifle", true);
                animator.SetBool("isWeaponPistol", false);
                equipedWeapon.weaponType = Weapon.Type.Rifle;
                break;
        }
        animator.SetFloat("ReloadSpeed", reloadRate);
    }

    // 무기 슬롯 입력부분
    public void WeaponInput()
    {
        // 스크롤로 받는 입력부분 입력이 있으면 실행
        if (input.scroll != 0 && !isGrenade)
        {
            int newSlot = weaponSlot;

            if (input.scroll > 0)
            {
                newSlot += 1;
                if (4 < newSlot) newSlot = 1;
            }
            else if (input.scroll < 0)
            {
                newSlot -= 1;
                if (0 >= newSlot) newSlot = 4;
            }

            switch (newSlot)
            {
                case 1:
                    input.weaponSlot1 = true;
                    break;
                case 2:
                    input.weaponSlot2 = true;
                    break;
                case 3:
                    input.weaponSlot3 = true;
                    break;
                case 4:
                    input.weaponSlot4 = true;
                    break;
            }

        }
    }
    IEnumerator Grenade()
    {
        yield return new WaitForSeconds(1.3f);

        GameObject newGrenade = PhotonNetwork.Instantiate(grenadePrefab.name, throwPosition.transform.position, Quaternion.identity);
        newGrenade.GetComponent<Rigidbody>().AddForce(calculateForce(), ForceMode.Impulse);
        newGrenade.GetComponent<Grenade>().setViewId(photonView.ViewID);

        yield return new WaitForSeconds(1f);

        grenade -= 1;   // 수류탄 개수 -1
        PlayerUIManager.instance.SetGrenade(grenade); // 수류탄 개수 추가
        isGrenade = false;
        state = State.Ready;

        // 수류탄 던지기 이전 슬롯으로 돌려주기
        int slotIs = weaponSlot;
        weaponSlot = 0;

        switch (slotIs)
        {
            case 1:
                input.weaponSlot1 = true;
                break;
            case 2:
                input.weaponSlot2 = true;
                break;
            case 3:
                input.weaponSlot3 = true;
                break;
            case 4:
                input.weaponSlot4 = true;
                break;
        }
    }
    // 던지는 힘 계산
    public Vector3 calculateForce()
    {
        return cameraSet.followCam.transform.forward * grenadePower;
    }
    public void SetWeapon(Weapon _tpsWeapon, Transform _fpsWeapon)
    {

        // 무기 장착 및 TPS IK 세팅
        equipedWeapon = _tpsWeapon;
        rightHandObj = equipedWeapon.rightHandObj.transform;     // 권총의 오른손 그랩
        leftHandObj = equipedWeapon.leftHandObj.transform;       // 권총의 왼손 그랩
        rightElbowObj = equipedWeapon.rightElbowObj.transform;   // 권총의 오른팔꿈치
        leftElbowObj = equipedWeapon.leftElbowObj.transform;     // 권총의 왼팔꿈치

        // 무기 장착 및 무기 정보 세팅
        switch ((Type)equipedWeapon.weaponType)
        {
            case Type.Pistol:
                animator.SetBool("isWeaponPistol", true);
                animator.SetBool("isWeaponRifle", false);
                equipedWeapon.weaponType = Weapon.Type.Pistol;
                break;
            case Type.Rifle:
                animator.SetBool("isWeaponRifle", true);
                animator.SetBool("isWeaponPistol", false);
                equipedWeapon.weaponType = Weapon.Type.Rifle;
                break;
        }
        damage = equipedWeapon.damage;
        reloadRate = equipedWeapon.reloadRate;
        fireRate = equipedWeapon.fireRate;
        animator.SetFloat("ReloadSpeed", reloadRate);

        PlayerUIManager.instance.SetAmmo(_tpsWeapon.ammo);           // 현재 탄 UI 세팅
        PlayerUIManager.instance.SetRemainingAmmo(_tpsWeapon.remainingAmmo); // 현재 남은 탄 UI 세팅
        PlayerUIManager.instance.SetGrenade(grenade);


        // FPS 애니메이션도 세팅
        handAnimator = _fpsWeapon.GetComponent<Animator>();
        handAnimator.SetFloat("ReloadSpeed", reloadRate);
        playerMovement.fpsAnimator = handAnimator;

        // FPS 파이어 포지션 변경해주기
        if (weaponSlot == 1 || weaponClass == WeaponClass.Commando && weaponSlot == 2)
        {
            fireTransform = _fpsWeapon.GetComponent<FireTransform>().fireTransform;
            bulletLineRenderer = _fpsWeapon.GetComponent<LineRenderer>();
            fireParticle = fireTransform.GetComponent<ParticleSystem>();

            // 사용할 점을 두개로 변경
            bulletLineRenderer.positionCount = 2;
            // 라인 렌더러를 비활성화
        }

        else
        {
            fireTransform = null;
            bulletLineRenderer = null;
            fireParticle = null;
        }
        // 무기의 상태도 체크
        if (0 < _tpsWeapon.ammo)
        { state = State.Ready; }
        else if (0 >= _tpsWeapon.ammo)
        { state = State.Empty; }
    }

    [PunRPC]
    public void GetAmmo(int value)
    {
        equipedWeapon.remainingAmmo += value;
        PlayerUIManager.instance.SetRemainingAmmo(equipedWeapon.remainingAmmo);
    }
    void HandSet()
    {
        weaponPosition.position = targetObj.position;
        weaponPosition.rotation = targetObj.rotation;
    }

    public void BuySMG()
    {
        fpsPistol.gameObject.SetActive(false);

        fpsPistol = fpsSMG; // 총변경
        tpsPistol = tpsSMG;

        isSMG = true;
        if(weaponSlot == 1)
        {
            fpsPistol.gameObject.SetActive(true);
            SetWeapon(tpsPistol, fpsPistol); // 무기 장착
            photonView.RPC("ServerProcessWeapon", RpcTarget.Others, 0);
            animator.SetBool("isWeaponPistol", true);
            animator.SetBool("isWeaponRifle", false);
        }
    }
    public void BuySCAR()
    {
        fpsRifle.gameObject.SetActive(false);

        fpsRifle = fpsSCAR; // 총변경
        tpsRifle = tpsSCAR;

        isSCAR = true;
        weaponClass = WeaponClass.Commando; // 클래스도 변경해주기

        if (weaponSlot == 2)
        {
            fpsRifle.gameObject.SetActive(true);
            SetWeapon(tpsRifle, fpsRifle); // 무기 장착
            photonView.RPC("ServerProcessWeapon", RpcTarget.Others, 1);

            animator.SetBool("isWeaponPistol", false);
            animator.SetBool("isWeaponRifle", true);
        }
    }

    // 무기 IK 애니메이션 처리
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // 플레이어 lookat
                if (targetObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(targetObj.position);
                }
                // 오른손 그랩
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                // 왼손 그랩
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }

                // 왼쪽 팔꿈치
                if (leftElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, elbowIKAmount);
                }
                // 오른쪽 팔꿈치
                if (rightElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, elbowIKAmount);
                }
            }
            // 그랩에 아무것도 없다면 0
            else
            {
                animator.SetLookAtWeight(0);

                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

                animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
            }
        }
    }
    //ssm 부모 찾기 
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