using System.Collections.Generic;
using UnityEngine;

public class PlayerPhsygunController : MonoBehaviour
{
    [Header("PhysGun Properties")]
    [SerializeField] float maxGrabDistance;
    [SerializeField] float minGrabDistance;
    [SerializeField] float scrollSpeed;
    [SerializeField] float dragSpeed;
    [SerializeField] Camera playerCamera;
    [SerializeField] LayerMask grabLayer;

    float grabDistance;
    Dictionary<Rigidbody, Rigidbody> jointSwaps = new();
    LineRenderer lineRenderer;
    Rigidbody selectedObject;
    Vector3 grabOffset;
    Quaternion rotationOffset;
    Vector3 selectedObjectPosition;
    Vector3 grabForce;

    private void Update()
    {
        KeyInputs();
        grabDistance = Mathf.Clamp(grabDistance + Input.mouseScrollDelta.y * scrollSpeed, minGrabDistance, maxGrabDistance);
        lineRenderer = GetComponent<LineRenderer>();
        Physics.SyncTransforms();
    }

    private void LateUpdate()
    {
        if (selectedObject)
        {
            var midpoint = playerCamera.transform.position + grabDistance * .5f * playerCamera.transform.forward;
            DrawQuadraticBezierCurve(lineRenderer, transform.position, midpoint, selectedObject.position + selectedObject.transform.TransformVector(grabOffset));
        }
    }

    private void FixedUpdate()
    {
        if (selectedObject != null)
        {
            var ray = Camera.main.ViewportPointToRay(Vector3.one * .5f);
            selectedObjectPosition = ray.origin + ray.direction * grabDistance - selectedObject.transform.TransformVector(grabOffset);
            var forceDir = selectedObjectPosition - selectedObject.position;
            grabForce = forceDir / Time.fixedDeltaTime * dragSpeed / selectedObject.mass; // 0.3f
            selectedObject.velocity = grabForce;
            // selectedObject.transform.rotation = playerCamera.transform.rotation * rotationOffset;    // set rotation of object based on camera | disabled to use manual rotation
        }
    }

    void KeyInputs()
    {
        if (Input.GetMouseButtonDown(0))
            Grab();
        if (Input.GetMouseButtonUp(0))
            if (selectedObject)
                Release();

        if (Input.GetKeyDown(KeyCode.R) && selectedObject != null)
            Release(true);

        if (selectedObject != null)
        {
            // Rotate left (Q key)
            if (Input.GetKey(KeyCode.Q))
                selectedObject.transform.Rotate(Vector3.up, 75f * Time.deltaTime);
            // Rotate right (E key)
            else if (Input.GetKey(KeyCode.E))
                selectedObject.transform.Rotate(Vector3.up, -75f * Time.deltaTime);
            // Face the player (R key)
            else if (Input.GetKeyDown(KeyCode.F))
            {
                selectedObject.transform.rotation = Quaternion.identity;
                var rb = selectedObject.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void Grab()
    {
        var ray = playerCamera.ViewportPointToRay(Vector3.one * .5f);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, grabLayer)
            && hit.rigidbody != null && !hit.rigidbody.CompareTag("Player"))
        {
            grabOffset = hit.transform.InverseTransformVector(hit.point - hit.transform.position);
            grabDistance = hit.distance;
            selectedObject = hit.rigidbody;
            // selectedObject.collisionDetectionMode = CollisionDetectionMode.Continuous;
            selectedObject.useGravity = false;
            selectedObject.isKinematic = false;
            lineRenderer.enabled = true;
        }
    }

    private void Release(bool freeze = false)
    {
        // selectedObject.collisionDetectionMode = CollisionDetectionMode.Continuous;
        selectedObject.useGravity = true;
        selectedObject.isKinematic = false;
        lineRenderer.enabled = false;

        if (freeze)
            Freeze(selectedObject);
        else
            Unfreeze(selectedObject);

        selectedObject = null;
    }

    private void Freeze(Rigidbody rb)
    {
        if (rb.TryGetComponent(out CharacterJoint characterJoint))
        {
            var fixedJointObject = GameObject.Instantiate(rb.gameObject, rb.transform.parent);
            var fixedJoint = fixedJointObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = characterJoint.connectedBody;
            fixedJoint.connectedAnchor = characterJoint.connectedAnchor;
            fixedJoint.massScale = characterJoint.massScale;
            fixedJoint.connectedMassScale = characterJoint.connectedMassScale;
            fixedJoint.GetComponent<Rigidbody>().isKinematic = true;
            jointSwaps.Add(fixedJoint.GetComponent<Rigidbody>(), rb);
            rb.gameObject.SetActive(false);
        }
        rb.isKinematic = true;
    }

    private void Unfreeze(Rigidbody rb)
    {
        if (jointSwaps.ContainsKey(rb))
        {
            jointSwaps[rb].gameObject.SetActive(true);
            jointSwaps[rb].isKinematic = false;
            jointSwaps[rb].transform.localPosition = rb.transform.localPosition;
            jointSwaps[rb].transform.localScale = rb.transform.localScale;
            jointSwaps[rb].transform.localRotation = rb.transform.localRotation;
            jointSwaps[rb].GetComponent<CharacterJoint>().connectedAnchor = rb.GetComponent<FixedJoint>().connectedAnchor;
            jointSwaps[rb].GetComponent<CharacterJoint>().anchor = rb.GetComponent<FixedJoint>().anchor;
            GameObject.Destroy(rb.gameObject);
            jointSwaps.Remove(rb);
        }
        else
            rb.isKinematic = false;
    }

    // line renderer curve
    void DrawQuadraticBezierCurve(LineRenderer line, Vector3 point0, Vector3 point1, Vector3 point2)
    {
        line.positionCount = 20;
        float t = 0f;
        Vector3 B = new(0, 0, 0);
        for (int i = 0; i < line.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            line.SetPosition(i, B);
            t += 1 / (float)line.positionCount;
        }
    }
}
