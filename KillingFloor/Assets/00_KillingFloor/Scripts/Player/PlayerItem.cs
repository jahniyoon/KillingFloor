using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    private PlayerShooter playerShooter;
    private PlayerInputs input;
    private GameObject nearObject;


    // Start is called before the first frame update
    void Start()
    {
        playerShooter = GetComponent<PlayerShooter>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    public void PlayerInput()
    {
        if(input.equip)
        {

            input.equip = false;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Item"))
        {
            nearObject = other.GetComponent<GameObject>();
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        nearObject = null;
    }
}
