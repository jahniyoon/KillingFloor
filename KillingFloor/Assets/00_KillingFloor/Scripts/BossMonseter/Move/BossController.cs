using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    private float bossHp = 3500f;//���� hp;
    private GameObject[] targetPlayer;
    private int randPlayerNum;//Ÿ���÷��̾�
    private Animator animator;
    private int randomFattern;//��������
    private float currentTime = 0f;
    private float setTime = 6.5f;
    private int saveFattern = 0;//��������
    private GameObject[] fireBreaths;//�극�� ������Ʈ �迭
    private ParticleSystem[] fireBreathsParticle;//�극�� ��ƼŬ �迭
    private GameObject[] fireBreathHoles;//���̷� ������Ʈ �迭
    private ParticleSystem[] fireBreathHoleParticles;//���̷� ��ƼŬ �迭
    private GameObject[] midSphereEffects;//���̷� ������Ʈ ������迭
    private ParticleSystem[] midSphereEffectParticles;//���̷� ��ƼŬ ������迭
    private GameObject[] fireRings;// ÷����� ������Ʈ �迭
    private ParticleSystem[] fireRingParticles;//������� ��ƼŬ ������迭
    private GameObject[] meteors;//���׿�


    private NavMeshAgent agent;
    private void Awake()
    {
        meteors = new GameObject[4];
        GameObject meteor = GameObject.Find("Meteor");



        for (int i = 0; i <= 1; i++) // ���׿� �迭�� �����ϴ°���
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

      
        for (int i = 0; i <= 2; i++) // ���� ��� �迭�� �����ϴ°���
        {
            fireRings[i] = fireRing.transform.GetChild(i).gameObject;
            fireRingParticles[i] = fireRings[i].GetComponent<ParticleSystem>();
            fireRings[i].SetActive(false);
        }
        for (int i = 0; i <= 1; i++) // ������ ��ƼŬ�迭�� �����ϴ°���
        {
            fireBreathHoles[i] = fireBreathHole.transform.GetChild(i).gameObject;
            fireBreathHoleParticles[i] = fireBreathHoles[i].GetComponent<ParticleSystem>();
            fireBreathHoles[i].SetActive(false);
        }
        for (int i = 0; i <= 1; i++) // ������ ������ ��ƼŬ�迭�� �����ϴ°���
        {
            midSphereEffects[i] = midSphere.transform.GetChild(i).gameObject;
            midSphereEffectParticles[i] = midSphereEffects[i].GetComponent<ParticleSystem>();
            midSphereEffects[i].SetActive(false);
        }

        for (int i = 0; i <= 3; i++) // �극�� ��ƼŬ�迭�� �����ϴ°���
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


        
            // �÷��̾�� ���� ������ �Ÿ��� ����մϴ�.
            float distance = Vector3.Distance(targetPlayer[randPlayerNum].transform.position, transform.position);
          
            if (distance < 6f)//�ٰŸ� �ִϸ��̼�
            {
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;
                agent.velocity = Vector3.zero;
             
                randomFattern = Random.Range(0, 6);
                if (randomFattern != saveFattern)// �������� �ߺ�üũ, �ߺ����� ����
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
                }// �������� �ߺ�üũ, �ߺ����� ���� End

                switch (randomFattern)//���� ��������
                {
                    case 0:

                        animator.SetTrigger("Attack1");//���
                        currentTime = 0;
                    
                        setTime = 3.7f;
                      
                        break;
                    case 1:

                        animator.SetTrigger("Attack2");//������
                        currentTime = 0;
                     
                        setTime = 3.7f;
                        break;
                    case 2:

                        animator.SetTrigger("Attack3");//�극��
                        for (int i = 0; i <= 2; i++)
                        {
                            fireBreaths[i].SetActive(true);
                            fireBreathsParticle[i].Play();
                        }
                        currentTime = 0;
                       
                        setTime = 5.75f;
                        break;
                    case 3:

                        animator.SetTrigger("Shout");//¢��
                        for (int i = 0; i <= 1; i++)
                        {
                            fireBreathHoles[i].SetActive(true);
                            fireBreathHoleParticles[i].Play();
                            midSphereEffects[i].SetActive(true);
                            if (i == 1)
                            {
                                midSphereEffectParticles[i].Play();
                                StartCoroutine(StopParticle(midSphereEffectParticles[i], 3.6f));//��ƼŬ ���� �ڷ�ƾ
                                StartCoroutine(FalseObj(midSphereEffects[0], 3.6f));//������Ʈ ���� �ڷ�ƾ
                            }
                            StartCoroutine(StopParticle(fireBreathHoleParticles[i], 3.6f));//��ƼŬ ���� �ڷ�ƾ


                        }
                        
                        currentTime = 0;
                        setTime = 3.6f;
                        break;
                    case 4://��������
                        animator.SetTrigger("Jump");
                        Invoke("JumpImpt", 1.1f);
                        currentTime = 0;                      
                        setTime = 3f;
                        break;
                    case 5://��극��
                        animator.SetTrigger("Breath");
                        Invoke("fireBreathImpt", 1f);
                        currentTime = 0;
                       
                        setTime = 5.8f;
                        break;
                  

                }
            }//���� ��������End
            else//���Ÿ� �ִϸ��̼�
            {
                animator.SetTrigger("Meteor");
                Invoke("fireBreathImpt", 1f);
                
                meteors[0].SetActive(true);
                currentTime = 0;

                setTime = 6f;


                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("idle"))// �ӵ� ��ȭ
                {
                    agent.isStopped = false;
                    agent.updatePosition = true;
                    agent.updateRotation = true;                    
                    BossMove();
                }
                animator.SetFloat("Speed", agent.speed);



            }//���Ÿ� �ִϸ��̼� end

        }

    }
    //����� �ٶ󺸴� ���� (������)
    private void LookRotate()
    {
        if (targetPlayer[randPlayerNum] == null) // ����������
        {
            targetPlayer = GameObject.FindGameObjectsWithTag("Player");
            randPlayerNum = Random.Range(0, targetPlayer.Length);
        }

        Vector3 lookDirection = targetPlayer[randPlayerNum].transform.position - transform.position;

        lookDirection = new Vector3(lookDirection.x, 0f, lookDirection.z);
        // ����� �ٶ󺸴� ȸ����(Quaternion) ���
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        // ���� ������Ʈ�� ȸ���� ����� �ٶ󺸴� ȸ������ ����
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 2 * Time.deltaTime);
    }
    //���� ������
    private void BossMove()
    {
        if (targetPlayer[randPlayerNum] != null)
        {
            agent.destination = targetPlayer[randPlayerNum].transform.position;
        }

    }
    //��ƼŬ ����
    private IEnumerator StopParticle(ParticleSystem particle, float num)
    {
        yield return new WaitForSeconds(num);
        particle.Stop();
 
    }
    //������Ʈ ��Ȱ��ȭ
    private IEnumerator FalseObj(GameObject gameObj, float num)
    {
        yield return new WaitForSeconds(num);
        gameObj.SetActive(false);

    }
    //���� ���
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
