using Photon.Pun;
using UnityEngine;

public class Meteor : MonoBehaviourPun
{
    private GameObject[] targetPlayer;
    private Transform orgPos;
    private float MeteorHP = 250;
    private GameObject meteorsField;
    private bool fildchk = false;
  
    private ParticleSystem[] meteorsParticle;//브레스 파티클 배열
                                             // Start is called before the first frame update

    void Start()
    {
       
        targetPlayer = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(MeteorHP + " ," + targetPlayer);
        MeteorHP = MeteorHP + (250 * targetPlayer.Length);
     
        meteorsField = GameObject.Find("FireBreathField");
      
        meteorsParticle = new ParticleSystem[5];

        

        for (int i = 0; i < 4; i++) // 메테오파티클 배열에 저장하는과정
        {
            meteorsParticle[i] = meteorsField.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();

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
            transform.position = new Vector3(orgPos.transform.position.x , orgPos.transform.position.y + 17.9f, orgPos.transform.position.z);
            gameObject.SetActive(false);
            return;
        }

      
        if (transform.position.y < orgPos.transform.position.y)
        {

            fildchk = true;
            
       
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
            if(!fildchk)
            {
               
                meteorsField.SetActive(false);
                fildchk = true;
                
            }
          
        }

        

     



    }
    // 데미지 
    public void OnDamage(float dam)
    {
        
        // 마스터에게 데미지 계산 요청
        Debug.Log("메테오 데미지 계산요청" + MeteorHP);

        photonView.RPC("MasterDamage", RpcTarget.MasterClient, dam);
    }
    // 2.마스터가 데미지 계산을 요청받고 계산을 먼저 해준다.
    // 계산이 끝난 값을 모두에게 보내준다.
    [PunRPC]
    public void MasterDamage(float _destroyCount)
    {
        Debug.Log("마스터 모두에게 데미지 업데이트 요청");
        MeteorHP -= _destroyCount;
     

        // 마스터가 계산한 값 전달
        photonView.RPC("SyncDamage", RpcTarget.All, MeteorHP);

    }
    // 3. 모두는 (마스터를 포함) 전달받은 값을 업데이트를 한다.
    [PunRPC]
    public void SyncDamage(float _destroyCount)
    {

        MeteorHP = _destroyCount;

    }
    
}
