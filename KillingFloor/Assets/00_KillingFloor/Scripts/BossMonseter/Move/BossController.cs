using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class BossController : MonoBehaviourPun
{
    public AudioClip intro;
    public AudioClip walk;
    public AudioClip jump;
    public AudioClip attack1Audio;
    public AudioClip attack2Audio;
    public AudioClip shoutAudio;
    public AudioClip fireBreathsAudio;
    public float bossHp = 3500f;//보스 hp;

    private AudioSource audioSource;
    private GameObject[] targetPlayer;
    private int randPlayerNum;//타겟플레이어
    private Animator animator;
    private int randomFattern;//랜덤패턴
    private float currentTime = 0f;
    private float setTime = 6f;
    private int saveFattern = 0;//이전패턴
    private GameObject[] fireBreaths;//브레스 오브젝트 배열
    private ParticleSystem[] fireBreathsParticle;//브레스 파티클 배열
    private GameObject[] fireBreathHoles;//사이렌 오브젝트 배열
    private ParticleSystem[] fireBreathHoleParticles;//사이렌 파티클 배열
    private GameObject[] midSphereEffects;//사이렌 오브젝트 베리어배열
    private ParticleSystem[] midSphereEffectParticles;//사이렌 파티클 베리어배열
    private GameObject[] fireRings;// 첨프충격 오브젝트 배열
    private ParticleSystem[] fireRingParticles;//점프충격 파티클 베리어배열
    private GameObject[] meteors;//메테오
    private int mereorCount = 0;
    private float[] meteorFattern;
    private NavMeshAgent agent;
    private bool dieChk = false;
    private Image Hpimage;
    private GameObject meteor;
    private GameObject bossintro;
    private bool changeBool = false;
    private bool audioChk = false;
    private bool atkChk = false;

    void Start()
    {
       
        meteors = new GameObject[4];
        meteor = GameObject.Find("Meteor");
      

      
        for (int i = 0; i <= 1; i++) // 메테오 배열에 저장하는과정
        {
            meteors[i] = meteor.transform.GetChild(i).gameObject;
            meteors[i].SetActive(false);
        }
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        fireBreathHoles = new GameObject[3];
        fireBreathHoleParticles = new ParticleSystem[3];
        fireBreaths = new GameObject[4];
        fireBreathsParticle = new ParticleSystem[4];
        midSphereEffects = new GameObject[4];
        midSphereEffectParticles = new ParticleSystem[4];
        fireRings = new GameObject[4];
        fireRingParticles = new ParticleSystem[4];
        bossintro = GameObject.Find("Bossintro");//보스 등장씬
        Hpimage = GameObject.Find("HP_Main").GetComponent<Image>();
        GameObject midSphere = GameObject.Find("MidSphereEffect");
        GameObject fireBreath = GameObject.Find("FireBreath");
        GameObject fireBreathHole = GameObject.Find("FireBreathHole");
        GameObject fireRing = GameObject.Find("FireRing");


        for (int i = 0; i <= 2; i++) // 점프 충격 배열에 저장하는과정
        {
            fireRings[i] = fireRing.transform.GetChild(i).gameObject;
            fireRingParticles[i] = fireRings[i].GetComponent<ParticleSystem>();
            fireRings[i].SetActive(false);
        }
        for (int i = 0; i <= 1; i++) // 샤우팅 파티클배열에 저장하는과정
        {
            fireBreathHoles[i] = fireBreathHole.transform.GetChild(i).gameObject;
            fireBreathHoleParticles[i] = fireBreathHoles[i].GetComponent<ParticleSystem>();
            fireBreathHoles[i].SetActive(false);
        }
        for (int i = 0; i <= 1; i++) // 샤우팅 베리어 파티클배열에 저장하는과정
        {
            midSphereEffects[i] = midSphere.transform.GetChild(i).gameObject;
            midSphereEffectParticles[i] = midSphereEffects[i].GetComponent<ParticleSystem>();
            midSphereEffects[i].SetActive(false);
        }

        for (int i = 0; i <= 3; i++) // 브레스 파티클배열에 저장하는과정
        {
            fireBreaths[i] = fireBreath.transform.GetChild(i).gameObject;
            fireBreathsParticle[i] = fireBreaths[i].GetComponent<ParticleSystem>();
            fireBreaths[i].SetActive(false);
        }

        animator = GetComponent<Animator>();
        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
        if (PhotonNetwork.IsMasterClient)
        {
            randPlayerNum = Random.Range(0, targetPlayer.Length);
        }
        bossHp = bossHp + (1000 * targetPlayer.Length);

        meteorFattern = new float[] { bossHp/2, 1000, 0, -10, -10, -10 };
    }

    void Update()
    {
        if(photonView.IsMine)
        {
            if (bossintro.activeSelf)
            {
                if (!audioChk)
                {
                  
                    StartCoroutine(MasterIntro());
                    audioChk = true;
                                   
                }
            }
        }

        if (!PhotonNetwork.IsMasterClient)
        { return; }

       

        if (bossHp <= 0)
        {
            if (dieChk == false)
            {
                animator.SetTrigger("Die");//죽음
                dieChk = true;
            }

            return;
        }

        //Debug.Log("현재 위치: " + transform.position + targetPlayer[randPlayerNum].transform.position + "네비 상대 위치 " + agent.destination);
        currentTime += Time.deltaTime;
        if (currentTime >= setTime)
        {
          

            if (targetPlayer[randPlayerNum].GetComponent<PlayerHealth>().dead)
            {
                changeplayerlook();
            }

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(targetPlayer[randPlayerNum].transform.position, path);
            agent.SetPath(path);
            // agent.SetDestination(targetPlayer[randPlayerNum].transform.position);

          

            if (!changeBool)
            {
                Invoke("changeplayerlook", 30f);
                changeBool = true;
            }
            if (!atkChk)
            {
                atkChk = true;
                randomFattern = Random.Range(0, 6);
                if (bossHp < meteorFattern[mereorCount])
                {
                    mereorCount++;
                    randomFattern = 6;
                }
            }
            
            Debug.Log(randomFattern);
            // 플레이어와 보스 사이의 거리를 계산합니다.
            float distance = Vector3.Distance(targetPlayer[randPlayerNum].transform.position, transform.position);
      
            if (distance < distanceAtk(randomFattern))//근거리 애니메이션
            {
                atkChk = false;
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
                agent.velocity = Vector3.zero;

                if (randomFattern != saveFattern)// 공격패턴 중복체크, 중복패턴 변경
                {
                    saveFattern = randomFattern;
                }
                else if (randomFattern == saveFattern)
                {
                    if (randomFattern != 0)
                    {
                        randomFattern = 0;
                        saveFattern = 0;
                    }
                    else
                    {
                        randomFattern = 1;
                        saveFattern = 1;
                    }
                }// 공격패턴 중복체크, 중복패턴 변경 End
              
                
                switch (randomFattern)//보스 공격패턴
                {
                    case 0:
                        animator.SetTrigger("Shout");//짖기
                        photonView.RPC("PartticleOn", RpcTarget.All, 2); // 포톤으로 호출

                        currentTime = 0;
                        setTime = 3.6f;

                        break;
                    case 1:

                        animator.SetTrigger("Attack2");//가르기
                        audioSource.PlayOneShot(attack2Audio);
                        currentTime = 0;

                        setTime = 3.7f;


                        break;
                    case 2:

                        StartCoroutine(TimeAudio(intro, 1));
                        AnimatorStart("Attack1");///찍기

                        audioSource.PlayOneShot(attack1Audio);
                        currentTime = 0;

                        setTime = 3.7f;
                    
                        break;
                    case 3:

                        animator.SetTrigger("Attack3");//브레스
                        photonView.RPC("PartticleOn", RpcTarget.All, 1); // 포톤으로 호출
                        audioSource.PlayOneShot(fireBreathsAudio);
                        currentTime = 0;

                        setTime = 5.75f;

                        break;
                    case 4://점프공격
                        audioSource.PlayOneShot(jump);
                        animator.SetTrigger("Jump");
                        photonView.RPC("PartticleOn", RpcTarget.All, 3); // 포톤으로 호출
                        currentTime = 0;
                        setTime = 3f;
                        break;
                    case 5://긴브레스
                        animator.SetTrigger("Breath");
                        audioSource.PlayOneShot(fireBreathsAudio);
                        photonView.RPC("PartticleOn", RpcTarget.All, 4); // 포톤으로 호출
                        currentTime = 0;

                        setTime = 5.8f;
                        break;
                    case 6://메테오
                        animator.SetTrigger("Meteor");

                        photonView.RPC("PartticleOn", RpcTarget.All, 5); // 포톤으로 호출

                        currentTime = 0;

                        setTime = 11f;
                        break;

                }


            }//보스 공격패턴End
            else//원거리 애니메이션
            {
               
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("idle"))// 속도 변화
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(walk);
                    }
                  
                    agent.isStopped = false;
                    agent.updatePosition = true;
                    agent.updateRotation = true;
             

                }
                animator.SetFloat("Speed", agent.speed);



            }//원거리 애니메이션 end

        }

    }
    // 랜덤공격 설정 
    private int distanceAtk(int num)
    {
        int returnNum = 0;
         switch(num)
        {
            case 0:
                returnNum = 3;
                break;
            case 1:
                returnNum = 4;
                break;
            case 2:
                returnNum = 5;
                break;
            case 3:
                returnNum = 7;
                break;
            case 4:
                returnNum = 6;
                break;
            case 5:
                returnNum = 8;
                break;
            case 6:
                returnNum = 20;
                break;
        }
        return returnNum;
    }

    [PunRPC]
    private void bossMove()// 보스 내비 설정 
    {
        agent.SetDestination(targetPlayer[randPlayerNum].transform.position);
    }
    [PunRPC]
    private void PartticleOn(int num) // 파티클 on/off 
    {
        switch(num)
        {
            case 1:
                Invoke("fireBreathImpt", 0.5f);
                break;
            case 2:
                for (int i = 0; i <= 1; i++)
                {
                    fireBreathHoles[i].SetActive(true);
                    fireBreathHoleParticles[i].Play();
                    midSphereEffects[i].SetActive(true);
                    if (i == 1)
                    {
                        midSphereEffectParticles[i].Play();
                        StartCoroutine(StopParticle(midSphereEffectParticles[i], 3.6f));//파티클 정지 코루틴
                        StartCoroutine(FalseObj(midSphereEffects[0], 3.6f));//오브젝트 정지 코루틴
                    }
                    StartCoroutine(StopParticle(fireBreathHoleParticles[i], 3.6f));//파티클 정지 코루틴


                }
                break;
            case 3:
                Invoke("JumpImpt", 1.1f);

                break;
            case 4:
                Invoke("fireBreathImpt", 1f);
                break;
            case 5:
                // 현재 오브젝트의 Transform 컴포넌트 가져오기
                Transform myTransform = transform;

                // 현재 오브젝트의 정면 방향 벡터 계산
                Vector3 forwardDirection = myTransform.forward;

                forwardDirection = new Vector3(forwardDirection.x, forwardDirection.y + 1f, forwardDirection.z);
                // 배치할 거리
                float distanceToPlace = 4.0f;

                // 오브젝트의 위치 계산 (현재 오브젝트의 위치에서 정면 방향으로 일정 거리만큼 이동)
                Vector3 newPosition = myTransform.position + forwardDirection * distanceToPlace;



                meteor.transform.position = newPosition;
                meteor.SetActive(true);

                // 오브젝트의 위치를 새로 계산한 위치로 설정
                //   meteors[i].transform.position = newPosition;
                // 오브젝트 활성화
                meteors[0].SetActive(true);

                break;
        }

    }
    private void AnimatorStart(string name)
    {
        animator.SetTrigger(name);
    }
  /*  //대상을 바라보는 로직 (사용안함)
    private void LookRotate()
    {
        if (targetPlayer[randPlayerNum] == null) // 대상사라질경우
        {
            targetPlayer = GameObject.FindGameObjectsWithTag("Player");
            randPlayerNum = Random.Range(0, targetPlayer.Length);
        }

        Vector3 lookDirection = targetPlayer[randPlayerNum].transform.position - transform.position;

        lookDirection = new Vector3(lookDirection.x, 0f, lookDirection.z);
        // 대상을 바라보는 회전값(Quaternion) 계산
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        // 현재 오브젝트의 회전을 대상을 바라보는 회전으로 설정
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 2 * Time.deltaTime);
    }
    //보스 움직임*/

    
    //파티클 정지
    private IEnumerator StopParticle(ParticleSystem particle, float num)
    {
        yield return new WaitForSeconds(num);
        audioSource.Stop();
        particle.Stop();

    }
    //오브젝트 비활성화
    private IEnumerator FalseObj(GameObject gameObj, float num)
    {
        yield return new WaitForSeconds(num);
        gameObj.SetActive(false);

    }
    
    //점프 충격
    private void JumpImpt()
    {
        for (int i = 0; i <= 2; i++)
        {
            fireRings[i].SetActive(true);
            fireRingParticles[i].Play();
            StartCoroutine(StopParticle(fireRingParticles[i], 1f));
        }

    }

    private void fireBreathImpt()
    {
        for (int i = 0; i <= 2; i++)
        {
            fireBreaths[i].SetActive(true);
            fireBreathsParticle[i].Play();
            StartCoroutine(StopParticle(fireBreathsParticle[i], 4f));
        }
    }
    private IEnumerator MasterIntro()
    {
        yield return new WaitForSeconds(4);
        int id = bossintro.GetComponent<PhotonView>().ViewID;

        photonView.RPC("bossIntroTime", RpcTarget.MasterClient, id); // 포톤으로 호출
      
       
    }

    [PunRPC]
    //보스 인트로
    private void bossIntroTime(int id)
    {

        photonView.RPC("IntroDel", RpcTarget.All, id); // 포톤으로 호출
    }

    [PunRPC]
    private void IntroDel(int id )
    {
        // Photon View ID를 사용하여 오브젝트 찾기
        GameObject targetObject = PhotonView.Find(id)?.gameObject;

        targetObject.SetActive(false);
    }
    public void bossHit(float dam)
    {

        Hpimage.fillAmount = normalization();
        bossHp -= dam;
    }
    // 정규화
    public float normalization()
    {
        float normalizedHealth = (bossHp - 0) / (3500f + (1000f * targetPlayer.Length) - 0);
        return normalizedHealth;
    }


    // 타겟변경
    public void changeplayerlook()
    {
       
        List<int> playerNumber = new List<int>();
        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i < targetPlayer.Length; i++)
        {            
            if(!targetPlayer[i].GetComponent<PlayerHealth>().dead)
            {
                playerNumber.Add(i);
            }
        }
        int targetNumber = Random.Range(0, playerNumber.Count);
        randPlayerNum = playerNumber[targetNumber];
        changeBool = false;
        photonView.RPC("bossMove", RpcTarget.All); // 포톤으로 호출
    }

    private IEnumerator TimeAudio(AudioClip audio ,float time)
    {
        yield return new WaitForSeconds(time);
        audioSource.PlayOneShot(audio);
    }
    // 데미지 
    public void OnDamage(float dam)
    {
        // 마스터에게 데미지 계산 요청
      

        photonView.RPC("MasterDamage", RpcTarget.MasterClient, dam);
    }
    // 2.마스터가 데미지 계산을 요청받고 계산을 먼저 해준다.
    // 계산이 끝난 값을 모두에게 보내준다.
    [PunRPC]
    public void MasterDamage(float _destroyCount)
    {
     

        bossHp -= _destroyCount;
      
        // 마스터가 계산한 값 전달
        photonView.RPC("SyncDamage", RpcTarget.All, bossHp);

    }
    // 3. 모두는 (마스터를 포함) 전달받은 값을 업데이트를 한다.
    [PunRPC]
    public void SyncDamage(float _destroyCount)
    {  
        Hpimage.fillAmount = normalization();
        bossHp = _destroyCount;

    }
}
