using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;
    public float smooth_time = 0.3f;
    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 target_position = target.position + Vector3.back;
        transform.position = Vector3.SmoothDamp(transform.position, target_position, ref velocity, smooth_time);
    }
}
