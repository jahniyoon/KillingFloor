using UnityEngine;

public class Meteor : MonoBehaviour
{
    Transform orgPos;
    private float MeteorHP = 300;
    GameObject meteorsField;

  
    private ParticleSystem[] meteorsParticle;//브레스 파티클 배열
                                             // Start is called before the first frame update

    void Start()
    {

       
        meteorsField = GameObject.Find("FireBreathField");
      
        meteorsParticle = new ParticleSystem[5];

        

        for (int i = 0; i < 4; i++) // 메테오파티클 배열에 저장하는과정
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
            //원래 위치로 이동
          
            return;
        }

      
        if (transform.position.y < orgPos.transform.position.y)
        {
          
            

       
            transform.position = new Vector3(orgPos.transform.position.x-9.51f, orgPos.transform.position.y +15f, orgPos.transform.position.z-2.25f);
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
}
