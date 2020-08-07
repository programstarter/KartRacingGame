using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidDetector : MonoBehaviour
{
    public float avoidPath = 0;
    public float avoidTime = 0;
    public float wanderDistance = 4;    //avoiding distance
    public float avoidLength = 1;   //1 sec

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "Car") return;

        Rigidbody otherCarRig = collision.rigidbody;
        avoidTime = Time.time + avoidLength;

        Vector3 otherCarLocalTaget = transform.InverseTransformPoint(otherCarRig.gameObject.transform.position);
        float otherCarAngle = Mathf.Atan2(otherCarLocalTaget.x, otherCarLocalTaget.z);
        avoidPath = wanderDistance * -Mathf.Sign(otherCarAngle);
    }
}
