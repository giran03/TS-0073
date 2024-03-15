using UnityEngine;

public class PlayerGrapplingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerController playerController;
    [SerializeField] LayerMask whatIsGrappleable;
    [SerializeField] Transform grappleOrigin;
    [SerializeField] Transform playerCam;
    [SerializeField] Transform player;

    [Header("Air Movement")]
    [SerializeField] Transform playerOrientation;
    [SerializeField] Rigidbody rb;
    [SerializeField] float horizontalThrustForce;
    [SerializeField] float forwardThrustForce;
    [SerializeField] float extendCableSpeed;

    [Header("Prediction")]
    [SerializeField] RaycastHit predictionHit;
    [SerializeField] float predictionSphereCastRadius;
    [SerializeField] float maxDistance;

    LineRenderer lr;
    Vector3 grapplePoint;
    Vector3 currentGrapplePosition;
    SpringJoint joint;
    bool sfxPlayed;

    private void Start() => lr = GetComponent<LineRenderer>();

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            StartGrapple();
        else if (Input.GetMouseButtonUp(1))
            StopGrapple();

        CheckForSwingPoints();
    }

    private void FixedUpdate()
    {
        if (joint != null) AirMovement();
    }

    void LateUpdate() => DrawRope();

    void AirMovement()
    {
        if (!playerController.isSwinging) return;

        // right thrust
        if (Input.GetKey(KeyCode.D)) rb.AddForce(horizontalThrustForce * Time.fixedDeltaTime * playerOrientation.right);
        // left thrust
        if (Input.GetKey(KeyCode.A)) rb.AddForce(horizontalThrustForce * Time.fixedDeltaTime * -playerOrientation.right);

        // forward thrust
        if (Input.GetKey(KeyCode.W)) rb.AddForce(horizontalThrustForce * Time.fixedDeltaTime * playerOrientation.forward);

        // shorten cable
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = grapplePoint - transform.position;
            rb.AddForce(forwardThrustForce * Time.fixedDeltaTime * directionToPoint.normalized);

            // calculat distance point a, b
            float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
        // extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, grapplePoint) + extendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }

    private void CheckForSwingPoints()
    {
        if (joint != null) return;

        Physics.SphereCast(playerCam.position, predictionSphereCastRadius, playerCam.forward, out RaycastHit sphereCastHit, maxDistance, whatIsGrappleable);

        Physics.Raycast(playerCam.position, playerCam.forward, out RaycastHit raycastHit, maxDistance, whatIsGrappleable);

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    void StartGrapple()
    {
        if (predictionHit.point == Vector3.zero) return;

        playerController.isSwinging = true;

        grapplePoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

        // the distance grapple will try to keep from grapple point. 
        joint.maxDistance = distanceFromPoint * .8f;
        joint.minDistance = distanceFromPoint * .25f;

        // joint settings
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = grappleOrigin.position;
    }

    void StopGrapple()
    {
        sfxPlayed = false;

        playerController.isSwinging = false;

        lr.positionCount = 0;
        Destroy(joint);
    }

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        // SFX
        if(!sfxPlayed)
        {
            sfxPlayed = true;
            AudioManager.Instance.PlaySFX("Grapple", transform.position);
        }

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, grappleOrigin.position);
        lr.SetPosition(1, currentGrapplePosition);
    }
}