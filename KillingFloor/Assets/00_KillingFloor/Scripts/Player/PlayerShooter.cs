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
        Ready, // �߻� �غ��
        Empty, // źâ�� ��
        Reloading // ������ ��
    }
    public State state { get; private set; }
    public enum Type { Pistol, Rifle, Melee, Heal };
    private PlayerInputs input;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private CameraSetup cameraSet;
    protected Animator animator;
    int layerMask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 14);    // ������ ���� ������ ���̾� ����ũ


    public Transform aimTarget; // �÷��̾ ���� ����
    public Transform targetObj;                // �÷��̾� ����
    public Transform weaponPosition = null;    // ���� ��ġ ������
    public Transform rightHandPosition; // ������ ��ġ

    [Header("Weapon Info")]
    public Weapon equipedWeapon;
    [Range(1, 5)]
    public int weaponSlot = 1;
    public float damage;        // �ѱ� ������
    public float range = 100f;  // ��Ÿ�
    public float reloadRate;    // ������ �ӵ�
    public float fireRate;      // ��� �ӵ�
    public float lastFireTime;  // ������ ��ݽð�
    public int grenade;         // ����ź ����
    public bool isGrenade;      // ����ź ���� üũ (1��Ī �ִϸ��̼�)
    public float healCoolDown = 15f;  // �� ��ٿ�

    public ParticleSystem muzzleFlashEffect; // �ѱ� ȭ��ȿ��
    public ParticleSystem shellEjectEffect;  // ź�� ���� ȿ��
    public AudioSource gunAudioPlayer;       // �� �Ҹ� �����

    public Transform fireTransform;          // �Ѿ��� �߻�� ��ġ
    public GameObject bulletHole;            // �Ѿ��� �´� ���� �����Ǵ� ��ƼŬ
    public GameObject bloodParticle;            // �Ѿ��� �´� ���� �����Ǵ� ��ƼŬ
    public LineRenderer bulletLineRenderer;  // �Ѿ� ������ �׸��� ���� ������
    public ParticleSystem fireParticle;
    public bool isParticleTrigger;          // ��ƼŬ �������� Ʈ����



    [Header("TPS Weapon")]
    Weapon tpsPistol;    // ������ ���� ���� ����
    Weapon tpsRifle;     // ������ ������ ���� ����
    Weapon tpsMelee;     // ������ ���� ���� ����
    Weapon tpsHeal;     // ������ ���� ���� ����

    [Header("FPS Weapon")]
    public Transform fpsPosition;
    public Transform fpsPistol;
    public Transform fpsRifle;
    public Transform fpsMelee;
    public Transform fpsHeal;
    public Transform fpsGrenade;

    [Header("Animator IK")]
    public Animator handAnimator;
    public Transform rightHandObj = null;   // ������
    public Transform leftHandObj = null;    // �޼�
    public Transform rightElbowObj = null;   // ������ �׷�
    public Transform leftElbowObj = null;    // �޼� �׷�
    [Range(0, 1)]
    public float handIKAmount = 1;
    [Range(0, 1)]
    public float elbowIKAmount = 1;
    [Range(0, 1)]
    public float animationIKAmount = 0.5f; // �ִϸ��̼� �� IK �⺻��
    public bool ikActive = false;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInputs>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        cameraSet = GetComponent<CameraSetup>();
        animator = GetComponent<Animator>();

        // TPS ���� ��������
        tpsPistol = weaponPosition.GetChild(0).GetComponent<Weapon>();
        tpsRifle = weaponPosition.GetChild(1).GetComponent<Weapon>();
        tpsMelee = weaponPosition.GetChild(2).GetComponent<Weapon>();
        tpsHeal = weaponPosition.GetChild(3).GetComponent<Weapon>();
        tpsRifle.gameObject.SetActive(false);    // �̸� ���α�
        tpsMelee.gameObject.SetActive(false);    // �̸� ���α�
        tpsHeal.gameObject.SetActive(false);    // �̸� ���α�

        // FPS ���� ��������
        fpsPosition = transform.GetChild(0).GetChild(0).GetComponent<Transform>();
        fpsPistol = fpsPosition.transform.GetChild(0).GetComponent<Transform>();
        fpsRifle = fpsPosition.transform.GetChild(1).GetComponent<Transform>();  // �������� �̸� �ҷ��ͼ� ���α�
        fpsMelee = fpsPosition.transform.GetChild(2).GetComponent<Transform>();
        fpsHeal = fpsPosition.transform.GetChild(3).GetComponent<Transform>();
        fpsGrenade = fpsPosition.transform.GetChild(4).GetComponent<Transform>();

        fpsRifle.gameObject.SetActive(false);
        fpsMelee.gameObject.SetActive(false);
        fpsHeal.gameObject.SetActive(false);
        fpsGrenade.gameObject.SetActive(false);


        SetWeapon(tpsPistol, fpsPistol); // ���� ����
        animator.SetBool("isWeaponPistol", true);
        animator.SetBool("isWeaponRifle", false);


        lastFireTime = 0;       // �ð� �ʱ�ȭ

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) { return; } // ���� �÷��̾ �ƴ� ��� �Է��� ���� �ʴ´�.

        // �Է� ���ɿ��� Ȯ��
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

    // �ֹ��� �������� ��� �Է�
    void Fire()
    {
        if (input.shoot && weaponSlot < 3)
        {
            // ���� ���°� �߻� ������ ����
            // && ������ �� �߻� �������� timeBetFire �̻��� �ð��� ����
            if (state == State.Ready && Time.time >= lastFireTime + fireRate && !input.dash && 0 < equipedWeapon.ammo)
            {
<<<<<<< HEAD
                // ������ �� �߻� ������ ����
                lastFireTime = Time.time;
                // ���� �߻� ó�� ����
                Shot();
=======
                GameObject hitObj = hit.transform.gameObject;
                Damage(hitObj); 
                hitPoint = hit.point;

>>>>>>> origin/feature/ssm
            }
            // ���� �Ѿ��� ���� �� �߻��ϸ� ������ ����
            else if (state == State.Empty && 0 < equipedWeapon.remainingAmmo && !input.dash)
            {
                input.shoot = false;
                input.reload = true; // ������ ��ư �����ֱ�
            }
            // ���� �Ѿ˵� ���� ��
            else if (equipedWeapon.ammo == 0 && equipedWeapon.remainingAmmo == 0 && !input.dash)
            {
                // ToDo : ƽ ���� �÷��̵ǵ��� �ϱ� (�Ѿ� ����)
                gunAudioPlayer.clip = equipedWeapon.emptyAudio;
                gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // �ѼҸ� ���
                input.shoot = false;
            }
        }

    }

    void Shot()
    {
        // ���� �߻� ó���� ȣ��Ʈ���� �븮
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient);
        
        // �ִϸ��̼� �۵� 
        handAnimator.SetTrigger("isFire");
        animator.SetTrigger("isFire");
        PlayerFireCameraShake.Invoke();
        equipedWeapon.ammo -= 1;
        PlayerUIManager.instance.SetAmmo(equipedWeapon.ammo);           // ���� ź UI ����

        if (equipedWeapon.ammo <= 0)
        {
            // źâ�� ���� ź���� ���ٸ�, ���� ���� ���¸� Empty���� ����
            state = State.Empty;
            input.shoot = false;
        }

    }
    // ȣ��Ʈ���� ����Ǵ� ���� �߻� ó��
    [PunRPC]
    private void ShotProcessOnServer()
    {
        // ����ĳ��Ʈ�� ���� �浹 ������ �����ϴ� ��������
        RaycastHit hit;
        Vector3 hitPoint = cameraSet.followCam.transform.forward * 1f;

        // ��� IK Ǯ���ֱ�
        handIKAmount = animationIKAmount;
        elbowIKAmount = animationIKAmount;
        StartCoroutine(ShootCoroutine());

        // ���� ������� ������ ������
        if (Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range, layerMask))
        {
            GameObject hitObj = hit.transform.gameObject;
            Damage(hitObj);
            hitPoint = hit.point;
        }
        // ���� ������ ������ �װ��� ��Ʈ����Ʈ��
        else if(Physics.Raycast(cameraSet.followCam.transform.position, cameraSet.followCam.transform.forward, out hit, range))
        {
            hitPoint = hit.point;
            isParticleTrigger = true;
        }
        // �ȴ����� �ִ�Ÿ��� ��Ʈ����Ʈ��
        else
        hitPoint = cameraSet.followCam.transform.forward * range;

        // ����Ʈ ��� �ڷ�ƾ�� ����
        photonView.RPC("ShotEffectProcessOnClients", RpcTarget.All, hitPoint);
 
        //aimTarget.transform.position = hitPoint;    // �÷��̾� ���� ������
    }
    // ����Ʈ ��� �ڷ�ƾ
    [PunRPC]
    private void ShotEffectProcessOnClients(Vector3 hitPosition)
    {
        StartCoroutine(ShotEffect(hitPosition));
    }
    // �߻� ����Ʈ�� �Ҹ��� ����ϰ� �Ѿ� ������ �׸���.
    private IEnumerator ShotEffect(Vector3 _hitPosition)
    {
        if (isParticleTrigger)
        { 
            // �Ѿ� �ڱ� ��ƼŬ ����
            GameObject particles = (GameObject)Instantiate(bulletHole);
            particles.transform.position = _hitPosition;
            Destroy(particles, 8f);
            isParticleTrigger = false;
        }
        fireParticle.Play();    // ��ƼŬ ���
        gunAudioPlayer.clip = equipedWeapon.gunAudio;
        gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // �ѼҸ� ���

        // ���� �������� �ѱ��� ��ġ
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        // ���� ������ �Է����� ���� �浹 ��ġ
        bulletLineRenderer.SetPosition(1, _hitPosition);
        // ���� �������� Ȱ��ȭ�Ͽ� �Ѿ� ������ �׸���
        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.01f);
        // ���η����� ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;
    }

    // �÷��̾� ����Ű �Է�
    void Aim()
    {
        // ������� ���� ���� �ִϸ��̼� False
        if (input.dash)
        {
            if (weaponSlot <= 2)
            {
                handAnimator.SetBool("isAim", false);
            }
            return; 
        }

        // �ֹ���, �������� ������ ���̰ų� ������� �ƴ� �� ����
        if (weaponSlot <= 2 && state == State.Ready && !input.dash)
        {
            handAnimator.SetBool("isAim", input.aim);
        }

        // �и� ��������̸� ������ ����
        if (weaponSlot == 3 && state == State.Ready && input.aim)
        {
            input.aim = false;
            handAnimator.SetTrigger("isAim");
            gunAudioPlayer.clip = equipedWeapon.reloadAudio;
            gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // �������� �Ҹ� ���
            StartCoroutine(WeaponDelay(reloadRate * 2));
        }
    }
    // ��� �����̸� �ֱ����� �ڷ�ƾ
    IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(fireRate); // fireRate �� RPM
        // �ܹ� ����
        if (weaponSlot == 1)
        {
            input.shoot = false;
        }
        handIKAmount = 1f;
        elbowIKAmount = 1f;
    }

    // �и� �Ǵ� ���� �����̸� �ֱ����� �ڷ�ƾ
    IEnumerator WeaponDelay(float _reloadRate)
    {
        yield return new WaitForSeconds(_reloadRate);
        state = State.Ready;
    }

    void Melee()
    {
        // ��������
        if (input.shoot && weaponSlot == 3 && state == State.Ready && !input.dash)
        {
            state = State.Reloading;
            handAnimator.SetTrigger("isFire");
            gunAudioPlayer.clip = equipedWeapon.gunAudio;
            gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // �������� �Ҹ� ���

            StartCoroutine(WeaponDelay(reloadRate));
            input.shoot = false;
        }
    }
    void Heal()
    {
        // �� Ŭ��
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

        // ��ٿ� ������Ʈ
        if (healCoolDown <= 15)
        {
            healCoolDown += Time.deltaTime;
            PlayerUIManager.instance.SetHeal(healCoolDown);
        }
    }

    void Damage(GameObject _hitObj)
    {
       
        if (!"Mesh_Alfa_2".Equals(FindTopmostParent(_hitObj.transform).gameObject.name)&& !"Meteor".Equals(FindTopmostParent(_hitObj.transform).gameObject.name))//���� �� �ƴҰ�� 
        {
<<<<<<< HEAD
            playerHealth.GetCoin(100);  // Debug ����׿� ��ȭ ȹ��
            return;
        }
        // ������ ���
        else if (_hitObj.transform.GetComponent<HitPoint>() != null)
        {
            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
            {
                _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // ���񿡰� ������

                // ���� ���� �״´ٸ�
                if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
                {
                    // ���� ���̰�
                    playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                    // ���ΰ� �ʱ�ȭ
                    _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                    //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
                }
=======
           
            ////////////////////////////////////////////////����////////////////////

            if (_hitObj.transform.GetComponent<HitPoint>() == null)
            {
                playerHealth.GetCoin(100);  // Debug ����׿� ��ȭ ȹ��
                return;
>>>>>>> origin/feature/ssm
            }
          
            if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health > 0)
            {
                _hitObj.transform.GetComponent<HitPoint>().Hit(damage); // ���񿡰� ������

                // ���� ���� �״´ٸ�
                if (_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().health <= 0)
                {
                    // ���� ���̰�
                    playerHealth.GetCoin(_hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin);

                    // ���ΰ� �ʱ�ȭ
                    _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin = 0;
                    //coin += _hitObj.transform.GetComponent<HitPoint>().parentObject.GetComponent<NormalZombie>().coin;
                }
            }

            ////////////////////////////////////////////////////////////////////
        }
      
        if ("Mesh_Alfa_2".Equals(FindTopmostParent(_hitObj.transform).gameObject.name)) // ���� �ϰ��
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
        // ������ ���
        if (_hitObj.transform.GetComponent<BossController>() != null)
        {
            // ���� ������ �־���ϴ� �κ�
            //_hitObj.transform.GetComponent<BossController>().bossHp -= damage;
        }
    }

    // ����
    public void Reload()
    {
        // �ֹ���, ���������� �� ���� ��ư�� �����ٸ�
        if (input.reload && weaponSlot < 3)
        {
            if (Reloading())
            {
                // �ִϸ��̼� �۵� �� ��� IK Ǯ���ֱ�
                handAnimator.SetTrigger("isReload");
                animator.SetTrigger("isReload");
                handIKAmount = animationIKAmount;
                elbowIKAmount = animationIKAmount;
                gunAudioPlayer.clip = equipedWeapon.reloadAudio;
                gunAudioPlayer.PlayOneShot(gunAudioPlayer.clip); // ������ �Ҹ� ���

                input.shoot = false;
                input.dash = false;
            }
            input.reload = false;
        }
    }
    private bool Reloading()
    {
        // �ܿ� ź�� 0���� ����, ź�� �������� �ʰ�, ���� ������ �� ����
        if(state == State.Reloading || equipedWeapon.remainingAmmo <= 0 || equipedWeapon.ammo == equipedWeapon.magazineSize)
        {
            return false;
        }
        StartCoroutine(ReloadCoroutine());
        return true;
    }
    // ���� �ڷ�ƾ
    IEnumerator ReloadCoroutine()
    {
        state = State.Reloading;

        yield return new WaitForSeconds(reloadRate);

        float currentAmmo = equipedWeapon.ammo;
        float remainingAmmo = equipedWeapon.remainingAmmo - (equipedWeapon.magazineSize - currentAmmo);

        equipedWeapon.ammo = Mathf.Min(equipedWeapon.magazineSize, equipedWeapon.ammo + equipedWeapon.remainingAmmo);   // ���� ź ����
        equipedWeapon.remainingAmmo = Mathf.Max(0, remainingAmmo);                                                      // ���� ź ����
        PlayerUIManager.instance.SetAmmo(equipedWeapon.ammo);           // ���� ź UI ����
        PlayerUIManager.instance.SetRemainingAmmo(equipedWeapon.remainingAmmo);           // ���� ź UI ����

        handIKAmount = 1f;
        elbowIKAmount = 1f;
        state = State.Ready;
    }

    // ȭ�鿡 �������� ���⸦ �����ϴ� �޼���
    public void Weapons()
    {
        // �غ�Ǿ��� ���� ���� ����
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
                SetWeapon(tpsPistol, fpsPistol); // ���� ����
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
                SetWeapon(tpsRifle, fpsRifle); // ���� ����
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
                SetWeapon(tpsMelee, fpsMelee); // ���� ����

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
                SetWeapon(tpsHeal, fpsHeal); // ���� ����

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

    // ���� ���� �Էºκ�
    public void WeaponInput()
    {
        // ��ũ�ѷ� �޴� �Էºκ�
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

        // ���� ���� �� TPS IK ����
        equipedWeapon = _tpsWeapon;
        rightHandObj = equipedWeapon.rightHandObj.transform;     // ������ ������ �׷�
        leftHandObj = equipedWeapon.leftHandObj.transform;       // ������ �޼� �׷�
        rightElbowObj = equipedWeapon.rightElbowObj.transform;   // ������ �����Ȳ�ġ
        leftElbowObj = equipedWeapon.leftElbowObj.transform;     // ������ ���Ȳ�ġ

        // ���� ���� �� ���� ���� ����
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

        PlayerUIManager.instance.SetAmmo(_tpsWeapon.ammo);           // ���� ź UI ����
        PlayerUIManager.instance.SetRemainingAmmo(_tpsWeapon.remainingAmmo); // ���� ���� ź UI ����
        PlayerUIManager.instance.SetGrenade(grenade);


        // FPS �ִϸ��̼ǵ� ����
        handAnimator = _fpsWeapon.GetComponent<Animator>();
        handAnimator.SetFloat("ReloadSpeed", reloadRate);
        playerMovement.fpsAnimator = handAnimator;

        // FPS ���̾� ������ �������ֱ�
        if (weaponSlot < 3)
        {
            fireTransform = _fpsWeapon.GetComponent<FireTransform>().fireTransform;
            bulletLineRenderer = _fpsWeapon.GetComponent<LineRenderer>();
            fireParticle = fireTransform.GetComponent<ParticleSystem>();

            // ����� ���� �ΰ��� ����
            bulletLineRenderer.positionCount = 2;
            // ���� �������� ��Ȱ��ȭ
        }

        else
        {
            fireTransform = null;
            bulletLineRenderer = null;
            fireParticle = null;
        }
        // ������ ���µ� üũ
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

    // ���� IK �ִϸ��̼� ó��
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // �÷��̾� lookat
                if (targetObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(targetObj.position);
                }
                // ������ �׷�
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
                // �޼� �׷�
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKAmount);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
              
                // ���� �Ȳ�ġ
                if (leftElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, elbowIKAmount);
                }
                // ������ �Ȳ�ġ
                if (rightElbowObj != null)
                {
                    animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowObj.position);
                    animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, elbowIKAmount);
                }
            }
            // �׷��� �ƹ��͵� ���ٸ� 0
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
    //ssm �θ� ã�� 
    private Transform FindTopmostParent(Transform currentTransform)
    {
        if (currentTransform.parent == null)
        {
            // ���� Transform�� ��Ʈ�̸� �ֻ��� �θ��̹Ƿ� ��ȯ�մϴ�.
            return currentTransform;
        }
        else
        {
            // �θ� ������ �θ��� �θ� ��������� ã���ϴ�.
            return FindTopmostParent(currentTransform.parent);
        }
    }
}