using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private GameObject[] targetPlayer;
    private int randPlayerNum;//Ÿ���÷��̾�
    private Animator animator;
    private int randomFattern;//��������
    private float currentTime = 0f;
    private float setTime = 6.5f;
    private int saveFattern = 0;//��������
    private float speed = 3f;//�����ȴ¾ִϸ��̼�
    private float bossSpeed = 0f;
    private GameObject[] fireBreaths;//�극�� ������Ʈ �迭
    private ParticleSystem[] fireBreathsParticle;//�극�� ��ƼŬ �迭
    private GameObject[] fireBreathHoles;//���̷� ������Ʈ �迭
    private ParticleSystem[] fireBreathHoleParticles;//���̷� ��ƼŬ �迭
    private GameObject[] midSphereEffects;//���̷� ������Ʈ ������迭
    private ParticleSystem[] midSphereEffectParticles;//���̷� ��ƼŬ ������迭
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

        for (int i=0; i<=3; i++) // �극�� ��ƼŬ�迭�� �����ϴ°���
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

            // �÷��̾�� ���� ������ �Ÿ��� ����մϴ�.
            float distance = Vector3.Distance(targetPlayer[randPlayerNum].transform.position, transform.position);
            speed = (distance/2)*2;
            if (distance < 6f)//�ٰŸ� �ִϸ��̼�
            {   
                randomFattern = Random.Range(0, 4);
                if(randomFattern != saveFattern)// �������� �ߺ�üũ, �ߺ����� ����
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
                        for(int i =0; i <= 2; i++)
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
                            if(i == 1)
                            {
                                midSphereEffectParticles[i].Play();
                                StartCoroutine(StopParticle(midSphereEffectParticles[i], 3.6f));//��ƼŬ ���� �ڷ�ƾ
                                StartCoroutine(FalseObj(midSphereEffects[0], 3.6f));//������Ʈ ���� �ڷ�ƾ
                            }                       
                            StartCoroutine(StopParticle(fireBreathHoleParticles[i], 3.6f));//��ƼŬ ���� �ڷ�ƾ
                          

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
            }//���� ��������End
            else//���Ÿ� �ִϸ��̼�
            {                            
                animator.SetFloat("Speed", speed);
                LookRotate();
                BossMove();


            }//���Ÿ� �ִϸ��̼� end

        }

    }
    //����� �ٶ󺸴� ���� 
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
   
}
