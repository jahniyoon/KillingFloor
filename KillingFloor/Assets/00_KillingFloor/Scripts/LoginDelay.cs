using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginDelay : MonoBehaviour
{
    public GameObject LoginPannel;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoginReady", 2f);
    }
    public void LoginReady()
    {
        LoginPannel.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
