
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class BossController : MonoBehaviour
{
    public float bossHp = 3500f;//보스 hp;
    private GameObject[] targetPlayer;
    private int randPlayerNum;//타겟플레이어
    private Animator animator;
    private int randomFattern;//랜덤패턴
    private float currentTime = 0f;
    private float setTime = 6.5f;
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
    private void Awake()
    {
        meteorFattern = new float[] { 2000, 1500, 0, -10, -10, -10 };
        meteors = new GameObject[4];
        meteor = GameObject.Find("Meteor");



        for (int i = 0; i <= 1; i++) // 메테오 배열에 저장하는과정
        {
            meteors[i] = meteor.transform.GetChild(i).gameObject;
            meteors[i].SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

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
        randPlayerNum = Random.Range(0, targetPlayer.Length);

    }

    // Update is called once per frame
    void Update()
    {
        if (bossintro.activeSelf)
        {
            Invoke("bossIntroTime", 4f);
        }
        if (bossHp <= 0)
        {
            if (dieChk == false)
            {
                animator.SetTrigger("Die");//죽음
                dieChk = true;
            }

            return;
        }

        currentTime += Time.deltaTime;
        if (currentTime >= setTime)
        {



            // 플레이어와 보스 사이의 거리를 계산합니다.
            float distance = Vector3.Distance(targetPlayer[randPlayerNum].transform.position, transform.position);

            if (distance < 6f)//근거리 애니메이션
            {
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
                agent.velocity = Vector3.zero;

                randomFattern = Random.Range(0, 6);
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

                if (bossHp < meteorFattern[mereorCount])
                {
                    mereorCount++;
                    randomFattern = 6;
                }
                switch (randomFattern)//보스 공격패턴
                {
                    case 0:

                        animator.SetTrigger("Attack1");//찍기
                        currentTime = 0;

                        setTime = 3.7f;

                        break;
                    case 1:

                        animator.SetTrigger("Attack2");//가르기
                        currentTime = 0;

                        setTime = 3.7f;
                        break;
                    case 2:

                        animator.SetTrigger("Attack3");//브레스
                        for (int i = 0; i <= 2; i++)
                        {
                            fireBreaths[i].SetActive(true);
                            fireBreathsParticle[i].Play();
                        }
                        currentTime = 0;

                        setTime = 5.75f;
                        break;
                    case 3:

                        animator.SetTrigger("Shout");//짖기
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

                        currentTime = 0;
                        setTime = 3.6f;
                        break;
                    case 4://점프공격
                        animator.SetTrigger("Jump");
                        Invoke("JumpImpt", 1.1f);
                        currentTime = 0;
                        setTime = 3f;
                        break;
                    case 5://긴브레스
                        animator.SetTrigger("Breath");
                        Invoke("fireBreathImpt", 1f);
                        currentTime = 0;

                        setTime = 5.8f;
                        break;
                    case 6://메테오
                        animator.SetTrigger("Meteor");
                        // 현재 오브젝트의 Transform 컴포넌트 가져오기
                        Transform myTransform = transform;

                        // 현재 오브젝트의 정면 방향 벡터 계산
                        Vector3 forwardDirection = myTransform.forward;

                        forwardDirection = new Vector3(forwardDirection.x, forwardDirection.y+1f, forwardDirection.z);
                        // 배치할 거리
                        float distanceToPlace = 4.0f;

                        // 오브젝트의 위치 계산 (현재 오브젝트의 위치에서 정면 방향으로 일정 거리만큼 이동)
                        Vector3 newPosition = myTransform.position + forwardDirection * distanceToPlace;



                        meteor.transform.position = newPosition;
                        meteor.SetActive(true);
                        for (int i = 0; i <= 1; i++)
                         {
                             // 오브젝트의 위치를 새로 계산한 위치로 설정
                          //   meteors[i].transform.position = newPosition;
                             // 오브젝트 활성화
                             meteors[i].SetActive(true);

                         }

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
                    agent.isStopped = false;
                    agent.updatePosition = true;
                    agent.updateRotation = true;
                    BossMove();
                }
                animator.SetFloat("Speed", agent.speed);



            }//원거리 애니메이션 end

        }

    }
    //대상을 바라보는 로직 (사용안함)
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
    //보스 움직임
    private void BossMove()
    {
        if (targetPlayer[randPlayerNum] != null)
        {
            agent.destination = targetPlayer[randPlayerNum].transform.position;
        }

    }
    //파티클 정지
    private IEnumerator StopParticle(ParticleSystem particle, float num)
    {
        yield return new WaitForSeconds(num);
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

    //보스 인트로
    private void bossIntroTime()
    {
        bossintro.SetActive(false);
    }
    public void bossHit(float dam)
    {

        Hpimage.fillAmount = normalization();
        bossHp -= dam;
    }
    public float normalization()
    {
        float normalizedHealth = (bossHp - 0) / (3500f - 0);
        return normalizedHealth;
    }
}
