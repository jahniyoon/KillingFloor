using UnityEngine;

public class Meteor : MonoBehaviour
{
    private Transform orgPos;
    public float MeteorHP = 500;
    private GameObject meteorsField;

  
    private ParticleSystem[] meteorsParticle;//�극�� ��ƼŬ �迭
                                             // Start is called before the first frame update

    void Start()
    {

       
        meteorsField = GameObject.Find("FireBreathField");
      
        meteorsParticle = new ParticleSystem[5];

        

        for (int i = 0; i < 4; i++) // ���׿���ƼŬ �迭�� �����ϴ°���
        {
            meteorsParticle[i] = meteorsField.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();

        }

        for (int i = 0; i < 4; i++)
        {
            meteorsParticle[i].Stop();
        }
        meteorsField.SetActive(false);

        orgPos = meteorsField.transform;

    }

    // Update is called once per frame
    void Update()
    {
        if(MeteorHP < 0)
        {
            //���� ��ġ�� �̵�
            transform.position = new Vector3(orgPos.transform.position.x , orgPos.transform.position.y + 17.9f, orgPos.transform.position.z);
            gameObject.SetActive(false);
            return;
        }

      
        if (transform.position.y < orgPos.transform.position.y)
        {
          
            

       
            transform.position = new Vector3(orgPos.transform.position.x, orgPos.transform.position.y + 17.9f, orgPos.transform.position.z);
            gameObject.SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                meteorsField.SetActive(true);

            }
            for (int i = 0; i < 4; i++)
            {
                meteorsParticle[i].Play();
            }
        }
        else
        {
            transform.Translate(Vector3.down * 2 * Time.deltaTime);
        }

        

     



    }

    public void MeteorHit(float dam)
    {
        Debug.Log(MeteorHP);
        MeteorHP -= dam;
    }
}
