using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        transform.position = target.position + offset;
        transform.eulerAngles = new Vector3(90f, target.eulerAngles.y, 0f);
    }
}
