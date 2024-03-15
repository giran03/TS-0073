using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBounds : MonoBehaviour
{
    (Vector3, Quaternion) initialPosition;
    private void Start() => initialPosition = (transform.position, transform.rotation);
    void Update()
    {
        if (transform.position.y < -30)
            RespawnObject();
    }

    void RespawnObject()
    {
        transform.SetPositionAndRotation(initialPosition.Item1, initialPosition.Item2);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Physics.SyncTransforms();
    }
}
