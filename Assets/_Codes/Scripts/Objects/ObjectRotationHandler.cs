using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectRotationHandler : MonoBehaviour
{
    [SerializeField] string[] groundLayers = new string[] {"Ground", "Grab", "Grapple"};
    LayerMask whatIsGround;

    private void Start() => whatIsGround = LayerMask.GetMask(groundLayers);

    private void Update()
    {
        var terrain = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, whatIsGround);

        if (terrain)
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation, 2f * Time.deltaTime);
            Physics.SyncTransforms();
        }
    }
}