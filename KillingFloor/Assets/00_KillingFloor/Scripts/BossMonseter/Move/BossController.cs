using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    private float bossHp = 3500f;//보스 hp;
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


    private NavMeshAgent agent;
    private void Awake()
    {
        meteors = new GameObject[4];
        GameObject meteor = GameObject.Find("Meteor");



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
        if(bossHp <= 0)
        {
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
                  

                }
            }//보스 공격패턴End
            else//원거리 애니메이션
            {
                animator.SetTrigger("Meteor");
                Invoke("fireBreathImpt", 1f);
                
                meteors[0].SetActive(true);
                currentTime = 0;

                setTime = 6f;


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
    private void OnAnimatorIK()
    {
        if(animator)
        {
            if (meteors[0]!= null)
            {
                animator.SetLookAtWeight(1);
                animator.SetLookAtPosition(meteors[0].transform.position);
            }
            else
            {
                animator.SetLookAtWeight(0);
            }
                
                
        }
    }

}
