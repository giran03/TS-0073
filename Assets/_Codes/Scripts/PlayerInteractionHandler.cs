using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Video;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] float grabDistance;
    [SerializeField] GameObject selectedObject;
    [SerializeField] Transform playerCamPosition;
    [SerializeField] LayerMask draggableLayer;
    GameObject lastSelectedObject;
    public static bool isDragging;
    public static float public_currentSelectedObject;
    RaycastHit hit;
    (Vector3, quaternion) initialPos;
    private void Update()
    {
        ObjectInteraction();
        ObjectBounds();

        Debug.DrawRay(playerCamPosition.position, playerCamPosition.TransformDirection(Vector3.forward) * grabDistance, Color.green);
    }
    private void ObjectInteraction()
    {
        if (Input.GetMouseButtonUp(0))
        {
            selectedObject = null;
            isDragging = false;

            if (lastSelectedObject != null)
            {
                Rigidbody tempRb = lastSelectedObject.GetComponent<Rigidbody>();
                tempRb.useGravity = true;
                tempRb.freezeRotation = false;
            }
        }

        if (Input.GetMouseButton(0))
        {
            // Debug.DrawRay(playerCamPosition.position, playerCamPosition.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            if (Physics.Raycast(playerCamPosition.position, playerCamPosition.TransformDirection(Vector3.forward), out hit, grabDistance, draggableLayer))
            {
                if (hit.collider != null)
                {
                    isDragging = true;
                    if (hit.collider.GetComponent<Rigidbody>())
                    {
                        if (selectedObject == null)
                        {

                            selectedObject = hit.collider.gameObject;
                            lastSelectedObject = selectedObject;
                            initialPos = (lastSelectedObject.transform.position, lastSelectedObject.transform.rotation);
                            public_currentSelectedObject = lastSelectedObject.transform.lossyScale.y;
                        }
                    }
                }
            }

            if (selectedObject != null)
            {
                Rigidbody tempRb = selectedObject.GetComponent<Rigidbody>();
                tempRb.useGravity = false;
                tempRb.freezeRotation = true;

                Vector3 selectedPos = selectedObject.transform.position;
                selectedPos.z = Mathf.Clamp(selectedPos.z, grabDistance, grabDistance);
                selectedObject.transform.position = selectedPos;

                SelectionControls();

                Physics.SyncTransforms();
            }
        }

        if (isDragging)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectedObject.transform.position.z));
            selectedObject.transform.position = pos;
        }
    }

    void SelectionControls()
    {
        if (selectedObject != null)
        {
            // Rotate left (Q key)
            if (Input.GetKey(KeyCode.Q))
                selectedObject.transform.Rotate(Vector3.up, 75f * Time.deltaTime);
            // Rotate right (E key)
            else if (Input.GetKey(KeyCode.E))
                selectedObject.transform.Rotate(Vector3.up, -75f * Time.deltaTime);
            // Face the player (R key)
            else if (Input.GetKeyDown(KeyCode.R))
                selectedObject.transform.rotation = Quaternion.identity;
        }
    }

    void ObjectBounds()
    {
        var (pos, rot) = initialPos;
        if (lastSelectedObject != null)
            if (lastSelectedObject.transform.position.y < -20)
            {
                lastSelectedObject.transform.SetPositionAndRotation(pos, rot);
                Rigidbody rb = lastSelectedObject.GetComponent<Rigidbody>();
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
            }
    }
}
