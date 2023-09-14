using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int value;
    public bool isBlink;
    public float duration;

    MeshRenderer renderer;
    Material mat;


    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        if(renderer != null)
        {
           mat = renderer.material;
        }
    }

    private void Update()
    {
        if(isBlink && renderer != null)
        {
            float emission = Mathf.PingPong(Time.time, duration);
            Color baseColor = new Color(0f,0.3f,0f,1f);
            Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

            mat.SetColor("_EmissionColor", finalColor);
        }
        
    }

}
