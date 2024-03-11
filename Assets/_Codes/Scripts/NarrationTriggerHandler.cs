using UnityEngine;
/// <summary>
/// This script can be found under 'SoundTrigger' prefab.
/// </summary>
public class NarrationTriggerHandler : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] Transform playerOrientation; // drop the playerOrientation under the playerObject prefab here

    AudioSource sfx;
    bool playerEntered;

    private void Start() => sfx = GetComponent<AudioSource>();

    private void Update()
    {
        if(!playerEntered) return;

        transform.position = playerOrientation.position; // Update the AudioSource's position to follow the player.
        if(!sfx.isPlaying)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioNarrationManager.Instance.QueueAudioClip(sfx.clip); // Add the clip to the queue.
            playerEntered = true;
        }
    }
}
