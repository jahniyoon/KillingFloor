using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;


public class PlayerShooter : MonoBehaviourPun
{
    public enum State
    {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }
    public State state { get; private set; }
    public enum Type { Pistol, Rifle, Melee, Heal };
    private PlayerInputs input;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private CameraSetup cameraSet;
    protected Animator animator;
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 14);    // 데미지 받을 좀비의 레이어 마스크


    public Transform aimTarget; // 플레이어가 보는 방향
    public Transform targetObj;                // 플레이어 시점
    public Transform weaponPosition = null;    // 무기 위치 기준점
    public Transform rightHandPosition; // 오른손 위치

    [Header("Weapon Info")]
    public Weapon equipedWeapon;
    [Range(1, 5)]
    public int weaponSlot = 1;
    public float damage;        // 총기 데미지
    public float range = 100f;  // 사거리
    public float reloadRate;    // 재장전 속도
    public float fireRate;      // 사격 속도
    public float lastFireTime;  // 마지막 사격시간
    public int grenade;         // 수류탄 개수
    public bool isGrenade;      // 수류탄 상태 체크 (1인칭 애니메이션)
    public float healCoolDown = 15f;  // 힐 쿨다운

    public ParticleSystem muzzleFlashEffect; // 총구 화염효과
    public ParticleSystem shellEjectEffect;  // 탄피 배출 효과
    public AudioSource gunAudioPlayer;       // 총 소리 재생기

    public Transform fireTransform;          // 총알이 발사될 위치
    public GameObject bulletHole;            // 총알이 맞는 곳에 생성되는 파티클
    public GameObject bloodParticle;            // 총알이 맞는 곳에 생성되는 파티클
    public LineRenderer bulletLineRenderer;  // 총알 궤적을 그리기 위한 렌더러
    public ParticleSystem fireParticle;
    public bool isParticleTrigger;          // 파티클 생성여부 트리거



    [Header("TPS Weapon")]
    Weapon tpsPistol;    // 가져올 권총 무기 정보
    Weapon tpsRifle;     // 가져올 라이플 무기 정보
    Weapon tpsMelee;     // 가져올 근접 무기 정보
    Weapon tpsHeal;     // 가져올 근접 무기 정보

    [Header("FPS Weapon")]
    public Transform fpsPosition;
    public Transform fpsPistol;
    public Transform fpsRifle;
    public Transform fpsMelee;
    public Transform fpsHeal;
    public Transform fpsGrenade;

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

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInputs>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        cameraSet = GetComponent<CameraSetup>();
        animator = GetComponent<Animator>();

        // TPS 무기 가져오기
        tpsPistol = weaponPosition.GetChild(0).GetComponent<Weapon>();
        tpsRifle = weaponPosition.GetChild(1).GetComponent<Weapon>();
        tpsMelee = weaponPosition.GetChild(2).GetComponent<Weapon>();
        tpsHeal = weaponPosition.GetChild(3).GetComponent<Weapon>();
        tpsRifle.gameObject.SetActive(false);    // 미리 꺼두기
        tpsMelee.gameObject.SetActive(false);    // 미리 꺼두기
        tpsHeal.gameObject.SetActive(false);    // 미리 꺼두기

        // FPS 무기 가져오기
        fpsPosition = transform.GetChild(0).GetChild(0).GetComponent<Transform>();
        fpsPistol = fpsPosition.transform.GetChild(0).GetComponent<Transform>();
        fpsRifle = fpsPosition.transform.GetChild(1).GetComponent<Transform>();  // 라이플은 미리 불러와서 꺼두기
        fpsMelee = fpsPosition.transform.GetChild(2).GetComponent<Transform>();
        fpsHeal = fpsPosition.transform.GetChild(3).GetComponent<Transform>();
        fpsGrenade = fpsPosition.transform.GetChild(4).GetComponent<Transform>();

        fpsRifle.gameObject.SetActive(false);
        fpsMelee.gameObject.SetActive(false);
        fpsHeal.gameObject.SetActive(false);
        fpsGrenade.gameObject.SetActive(false);


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
        if (GameManager.instance != null && GameManager.instance.inputEnable)
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
        if (input.shoot && weaponSlot < 3)
        {
            // 현재 상태가 발사 가능한 상태
            // && 마지막 총 발사 시점에서 timeBetFire 이상의 시간이 지남
            if (state == State.Ready && Time.time >= lastFireTime + fireRate && !input.dash && 0 < equipedWeapon.ammo)
            {
<<<<<<< HEAD
                // 마지막 총 발사 시점을 갱신
                lastFireTime = Time.time;
                // 실제 발사 처리 실행
                Shot();
=======
                GameObject hitObj = hit.transform.gameObject;
                Damage(hitObj); 
                hitPoint = hit.point;

>>>>>>> origin/feature/ssm
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

    }

    void Shot()
    {
        // 실제 발사 처리는 호스트에게 대리
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient);
        
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

    }
    // 호스트에서 실행되는 실제 발사 처리
    [PunRPC]
    private void ShotProcessOnServer()
    {
        // 레이캐스트에 의한 충돌 정보를 저장하는 컨테이터
        RaycastHit hit;
        Vector3 hitPoint = cameraSet.followCam.transform.forward * 1f;

        // 잠깐 IK 풀어주기
        handIKAmount = animationIKAmount;
        elbowIKAmount = animationIKAmount;
        StartCoroutine(ShootCoroutine());

        // 만약 좀비류에 닿으면 데미지
        if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range, layerMask))
        {
            GameObject hitObj = hit.transform.gameObject;
            Damage(hitObj);
            hitPoint = hit.point;
        }
        // 만약 뭔가에 닿으면 그곳을 히트포인트로
        else if(Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range))
        {
            hitPoint = hit.point;
            isParticleTrigger = true;
        }
        // 안닿으면 최대거리를 히트포인트로
        else
        hitPoint = cameraSet.followCam.transform.forward * range;

        // 이펙트 재생 코루틴을 랩핑
        photonView.RPC("ShotEffectProcessOnClients", RpcTarget.All, hitPoint);
 
        //aimTarget.transform.position = hitPoint;    // 플레이어 조준 포지션
    }
    // 이펙트 재생 코루틴
    [PunRPC]
    private void ShotEffectProcessOnClients(Vector3 hitPosition)
    {
        StartCoroutine(ShotEffect(hitPosition));
    }
    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다.
    private IEnumerator ShotEffect(Vector3 _hitPosition)
    {
        if (isParticleTrigger)
        { 
            // 총알 자국 파티클 생성
            GameObject particles = (GameObject)Instantiate(bulletHole);
            particles.transform.position = _hitPosition;
            Destroy(particles, 8f);
            isParticleTrigger = false;
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
            input.aim = false;
            handAnimator.SetTrigger("isAim");
            gunAudioPlayer.clip = equipedWeapon.reloadAudio;
            gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // 근접공격 소리 재생
            StartCoroutine(WeaponDelay(reloadRate * 2));
        }
    }
    // 사격 딜레이를 주기위한 코루틴
    IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(fireRate); // fireRate 는 RPM
        // 단발 설정
        if (weaponSlot == 1)
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
            playerHealth.RestoreHealth(heal);
            input.shoot = false;
        }

        // 쿨다운 업데이트
        if (healCoolDown <= 15)
        {
            healCoolDown += Time.deltaTime;
            PlayerUIManager.instance.SetHeal(healCoolDown);
        }
    }

    void Damage(GameObject _hitObj)
    {
       
        if (!"Mesh_Alfa_2".Equals(FindTopmostParent(_hitObj.transform).gameObject.name)&& !"Meteor".Equals(FindTopmostParent(_hitObj.transform).gameObject.name))//보스 가 아닐경우 
        {
<<<<<<< HEAD
            playerHealth.GetCoin(100);  // Debug 디버그용 재화 획득
            return;
        }
        // 좀비일 경우
        else if (_hitObj.transform.GetComponent<HitPoint>() != null)
        {
            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
            {
                _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // 좀비에게 데미지

                // 만약 좀비가 죽는다면
                if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
                {
                    // 코인 먹이고
                    playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                    // 코인값 초기화
                    _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                    //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
                }
=======
           
            ////////////////////////////////////////////////좀비////////////////////

            if (_hitObj.transform.GetComponent<HitPoint>() == null)
            {
                playerHealth.GetCoin(100);  // Debug 디버그용 재화 획득
                return;
>>>>>>> origin/feature/ssm
            }
          
            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
            {
                _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // 좀비에게 데미지

                // 만약 좀비가 죽는다면
                if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
                {
                    // 코인 먹이고
                    playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                    // 코인값 초기화
                    _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                    //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
                }
            }

            ////////////////////////////////////////////////////////////////////
        }
      
        if ("Mesh_Alfa_2".Equals(FindTopmostParent(_hitObj.transform).gameObject.name)) // 보스 일경우
        {


            if (9 == _hitObj.transform.gameObject.layer)
            {
               
                FindTopmostParent(_hitObj.transform).gameObject.GetComponent<BossController>().bossHit(damage);
            }
            else if (11 == _hitObj.transform.gameObject.layer)
            {
              
                FindTopmostParent(_hitObj.transform).gameObject.GetComponent<BossController>().bossHit(damage*0.5f);
            }
        }
    
        if ("Meteor".Equals(FindTopmostParent(_hitObj.transform).gameObject.name))
        {
           
               
                 
            _hitObj.gameObject.GetComponent<Meteor>().MeteorHit(damage);
            
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
        if(state == State.Reloading || equipedWeapon.remainingAmmo <= 0 || equipedWeapon.ammo == equipedWeapon.magazineSize)
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
        // 준비되었을 때만 변경 가능
        if (state == State.Ready && !input.dash)
        {
            if (input.weaponSlot1 && weaponSlot != 1 && !isGrenade)
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
                animator.SetBool("isWeaponPistol", true);
                animator.SetBool("isWeaponRifle", false);

                input.weaponSlot1 = false;
            }
            if (input.weaponSlot2 && weaponSlot != 2 && !isGrenade)
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
                animator.SetBool("isWeaponPistol", false);
                animator.SetBool("isWeaponRifle", true);

                input.weaponSlot2 = false;
            }
            if (input.weaponSlot3 && weaponSlot != 3 && !isGrenade)
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

                input.weaponSlot3 = false;
            }
            if (input.weaponSlot4 && weaponSlot != 4 && !isGrenade)
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

    // 무기 슬롯 입력부분
    public void WeaponInput()
    {
        // 스크롤로 받는 입력부분
        if (input.scroll != 0)
        {
            if (input.scroll > 0)
            {
                weaponSlot += 1;
                if (4 < weaponSlot) weaponSlot = 1;
            }
            else if (input.scroll < 0)
            {
                weaponSlot -= 1;
                if (0 >= weaponSlot) weaponSlot = 4;
            }
            switch (weaponSlot)
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
        yield return new WaitForSeconds(2.1f);
        grenade -= 1;
        PlayerUIManager.instance.SetGrenade(grenade);
        isGrenade = false;
        state = State.Ready;
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
        if (weaponSlot < 3)
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
        else if(0 >= _tpsWeapon.ammo)
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