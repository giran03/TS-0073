using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] Transform playerOrientation;
    [SerializeField] Transform playerCamPosition;
    [SerializeField] Transform firstPersonCamera;
    [SerializeField] float sensX;
    [SerializeField] float sensY;
    float xRotation;
    float yRotation;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        // follow player
        transform.position = playerCamPosition.position;

        // mouse look
        float mousex = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mousey = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mousex;
        xRotation -= mousey;


        xRotation = Mathf.Clamp(xRotation, -90, 90);

        // rotate cam and orientation
        firstPersonCamera.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
        playerCamPosition.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}