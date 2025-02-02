using Unity.VisualScripting;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}

public class PlayerObjectInteractions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LayerMask whatIsInteractable;
    [SerializeField] Transform playerCam;
    [SerializeField] GameObject buttonObj;


    [Header("Interact Settings")]
    [SerializeField] RaycastHit predictionHit;
    [SerializeField] float interactDistance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !Input.GetMouseButton(0))
            CheckForObjects();
    }

    private void CheckForObjects()
    {
        if (Physics.Raycast(playerCam.position, playerCam.forward, out RaycastHit raycastHit, interactDistance, whatIsInteractable))
            if (raycastHit.collider.gameObject.TryGetComponent(out IInteractable interactOjb))
                interactOjb.Interact();
    }
}