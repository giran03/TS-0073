using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTriggerHandler : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] Transform playerOrientation;
    AudioSource sfx;
    bool hasPlayed = false;
    bool playerEntered;

    private void Start()
    {
        sfx = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(!playerEntered) return;
        transform.position = playerOrientation.position; // Update the AudioSource's position to follow the player.
        if(!sfx.isPlaying)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            sfx.Play(); // Play the sound on the AudioSource.
            hasPlayed = true;
            playerEntered = true;
        }
    }
}
