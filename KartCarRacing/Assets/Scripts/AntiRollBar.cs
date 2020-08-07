﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    public float antiRoll = 5000.0f;
    public WheelCollider wheelLFront;
    public WheelCollider wheelRFront;
    public WheelCollider wheelLBack;
    public WheelCollider wheelRBack;
    public GameObject COM;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.centerOfMass = COM.transform.localPosition;
    }

    void FixedUpdate()
    {
        GroundWheels(wheelLFront, wheelRFront);
        GroundWheels(wheelLBack, wheelRBack);
    }

    void GroundWheels(WheelCollider WL, WheelCollider WR)
    {
        WheelHit wheelHit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = WL.GetGroundHit(out wheelHit);
        if (groundedL)
            travelL = (-WL.transform.InverseTransformPoint(wheelHit.point).y - WL.radius / WL.suspensionDistance);

        bool groundedR = WR.GetGroundHit(out wheelHit);
        if (groundedR)
            travelR = (-WR.transform.InverseTransformPoint(wheelHit.point).y - WR.radius / WR.suspensionDistance);

        float antiRollForce = (travelL - travelR) * antiRoll;

        if (groundedL)
            rb.AddForceAtPosition(WL.transform.up * -antiRollForce, WL.transform.position);

        if (groundedR)
            rb.AddForceAtPosition(WR.transform.up * antiRollForce, WR.transform.position);
    }
}
