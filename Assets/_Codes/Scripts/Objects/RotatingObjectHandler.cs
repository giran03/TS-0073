using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObjectHandler : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] float torque;
    [SerializeField] float maxAngularVelocity;
    [SerializeField] bool counterClockWise;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    void FixedUpdate()
    {
        if(counterClockWise)
            rb.AddTorque(-10f * torque * transform.up);
        else
            rb.AddTorque(10f * torque * transform.up);
    }
}
