using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{

    Camera main_camera;

    void Start()
    {
        main_camera = Camera.main;
    }

    void Update()
    {
        Vector3 lookDirection = transform.position - main_camera.transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = rotation;
    }
}
