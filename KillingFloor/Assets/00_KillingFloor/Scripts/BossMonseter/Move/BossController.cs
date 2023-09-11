using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private GameObject[] targetPlayer;
    private int randPlayerNum;//타겟플레이어
    private Animator animator;
    private int randomFattern;//랜덤패턴
    private float currentTime = 0f;
    private float setTime = 6.5f;
    private int saveFattern = 0;//이전패턴
    private float speed = 3f;//보스걷는애니메이션
    private float bossSpeed = 0f;
    private GameObject[] fireBreaths;//브레스 오브젝트 배열
    private ParticleSystem[] fireBreathsParticle;//브레스 파티클 배열
    private GameObject[] fireBreathHoles;//사이렌 오브젝트 배열
    private ParticleSystem[] fireBreathHoleParticles;//사이렌 파티클 배열
    private GameObject[] midSphereEffects;//사이렌 오브젝트 베리어배열
    private ParticleSystem[] midSphereEffectParticles;//사이렌 파티클 베리어배열
    // Start is called before the first frame update
    void Start()
    {
        fireBreathHoles = new GameObject[3];
        fireBreathHoleParticles = new ParticleSystem[3];
        fireBreaths = new GameObject[4];
        fireBreathsParticle = new ParticleSystem[4];
        midSphereEffects = new GameObject[4];
        midSphereEffectParticles = new ParticleSystem[4];

        GameObject midSphere = GameObject.Find("MidSphereEffect");
        GameObject fireBreath = GameObject.Find("FireBreath");
        GameObject fireBreathHole = GameObject.Find("FireBreathHole");
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

        for (int i=0; i<=3; i++) // 브레스 파티클배열에 저장하는과정
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
        currentTime += Time.deltaTime;
        if (currentTime >= setTime)
        {
            animator.SetTrigger("Idle");

            // 플레이어와 보스 사이의 거리를 계산합니다.
            float distance = Vector3.Distance(targetPlayer[randPlayerNum].transform.position, transform.position);
            speed = (distance/2)*2;
            if (distance < 6f)//근거리 애니메이션
            {   
                randomFattern = Random.Range(0, 4);
                if(randomFattern != saveFattern)// 공격패턴 중복체크, 중복패턴 변경
                {
                    saveFattern = randomFattern;
                }
                else if(randomFattern == saveFattern)
                {
                    if(randomFattern !=0)
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
                        for(int i =0; i <= 2; i++)
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
                            if(i == 1)
                            {
                                midSphereEffectParticles[i].Play();
                                StartCoroutine(StopParticle(midSphereEffectParticles[i], 3.6f));//파티클 정지 코루틴
                                StartCoroutine(FalseObj(midSphereEffects[0], 3.6f));//오브젝트 정지 코루틴
                            }                       
                            StartCoroutine(StopParticle(fireBreathHoleParticles[i], 3.6f));//파티클 정지 코루틴
                          

                        }

                        currentTime = 0;
                        setTime = 3.75f;
                        break;
                    case 4:
                   
                        animator.SetTrigger("Jump");
                        currentTime = 0;
                        setTime = 2;
                        break;
                }
            }//보스 공격패턴End
            else//원거리 애니메이션
            {                            
                animator.SetFloat("Speed", speed);
                LookRotate();
                BossMove();


            }//원거리 애니메이션 end

        }

    }
    //대상을 바라보는 로직 
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
        if(speed > 10)
        {
            bossSpeed = 14;
        }
        else
        {
            bossSpeed = 7;
        }
        Vector3 targetVector = transform.position - targetPlayer[randPlayerNum].transform.position;
        transform.Translate(Vector3.forward * bossSpeed * Time.deltaTime); ;
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
   
}
