using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public float angular_speed = 15;
    void Update()
    {
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles += Vector3.forward * (angular_speed * Time.deltaTime);
        transform.rotation = rotation;
    }
}
