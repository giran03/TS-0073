using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObjectHandler : MonoBehaviour
{
    [SerializeField] string[] layerMasks;
    LayerMask whatIsGround;
    private void Start()
    {
        whatIsGround = LayerMask.GetMask(layerMasks);
    }
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
