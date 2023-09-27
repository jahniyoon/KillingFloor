using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviourPun
{
    public MeshRenderer itemRenderer;
    Material mat;
    PlayerInputs input;
    PlayerShooter shooter;
    public GameObject equipUI;

    public int value;
    public bool isBlink;
    public float duration;

    void Start()
    {
        if(itemRenderer != null)
        {
           mat = itemRenderer.material;
        }
    }

    private void Update()
    {
        ItemBlink();
        
    }
    // 아이템 초록색으로 깜빡거리기
    public void ItemBlink()
    {
        if (isBlink && itemRenderer != null)
        {
            float emission = Mathf.PingPong(Time.time, duration);
            Color baseColor = new Color(0f, 0.3f, 0f, 1f);
            Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

            mat.SetColor("_EmissionColor", finalColor);
        }
    }

    private void OnTriggerStay(Collider player)
    {
        // 플레이어가 근처에 있으면
        if(player.CompareTag("Player"))
        {

            value =(int)player.GetComponent<PlayerShooter>().equipedWeapon.magazineSize;
            if((int)player.GetComponent<PlayerShooter>().equipedWeapon.maxAmmo < value + (int)player.GetComponent<PlayerShooter>().equipedWeapon.remainingAmmo)
            {
                value = (int)player.GetComponent<PlayerShooter>().equipedWeapon.maxAmmo - (int)player.GetComponent<PlayerShooter>().equipedWeapon.remainingAmmo;
            }

            input = player.GetComponent<PlayerInputs>();
            shooter = player.GetComponent<PlayerShooter>(); 
            equipUI.SetActive(true);

            if(input.equip)
            {
                shooter.photonView.RPC("GetAmmo", RpcTarget.All,value);
                //shooter.GetAmmo(value);
                equipUI.SetActive(false);
                input.equip = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit()
    {
        equipUI.SetActive(false);
    }

}
