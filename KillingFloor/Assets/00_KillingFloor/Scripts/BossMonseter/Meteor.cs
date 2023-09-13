using UnityEngine;

public class Meteor : MonoBehaviour
{
    Transform orgPos;
    private float MeteorHP = 300;
    GameObject meteorsField;

    private ParticleSystem[] meteorsParticle;//브레스 파티클 배열
                                             // Start is called before the first frame update
    private void Awake()
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
        orgPos = meteorsField.transform;
    }
    void Start()
    {
        Debug.Log(meteorsParticle.Length);


    }

    // Update is called once per frame
    void Update()
    {
        if(MeteorHP < 0)
        {
            //원래 위치로 이동
            transform.position =new Vector3(orgPos.position.x, orgPos.position.y + 15f,orgPos.position.z);
            gameObject.SetActive(false);
            return;
        }
    
         
        if (transform.position.y < orgPos.transform.position.y)
        {
            for (int i = 0; i < meteorsParticle.Length-1; i++)
            {
                meteorsField.SetActive(true);

            }
            for (int i = 0; i < meteorsParticle.Length-1; i++)
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
