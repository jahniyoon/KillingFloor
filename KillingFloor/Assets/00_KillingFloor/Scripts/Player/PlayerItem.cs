using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    private PlayerShooter shooter;
    private PlayerInputs input;
    public ItemObject nearObject;
    public int itemValue;


    // Start is called before the first frame update
    void Start()
    {
        shooter = GetComponent<PlayerShooter>();
        input = GetComponent<PlayerInputs>();
    }

    // Update is called once per frame
    void Update()
    {
    }

  
    public void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Item"))
        {
            nearObject = other.GetComponent<ItemObject>();
            PlayerUIManager.instance.equipUI.SetActive(true);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        PlayerUIManager.instance.equipUI.SetActive(false);
        nearObject = null;
    }
}
