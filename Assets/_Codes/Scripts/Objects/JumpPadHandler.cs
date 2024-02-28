using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadHandler : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpPadCooldownDuration;
    bool isOnCooldown;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isOnCooldown)
        {
            StartCoroutine(JumpPadCooldown());
            Rigidbody playerRb = other.gameObject.transform.parent.GetComponent<Rigidbody>();
            playerRb.AddForce(10f * jumpForce * Vector3.up, ForceMode.Impulse);
        }
    }

    IEnumerator JumpPadCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(jumpPadCooldownDuration);
        isOnCooldown = false;
    }
}
