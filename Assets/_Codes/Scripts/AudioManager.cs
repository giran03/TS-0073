using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script can be found under 'AudioManager' in the heirarchy.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } // Singleton instance.
    private AudioSource audioSource;
    private Queue<AudioClip> audioQueue = new();

    private void Awake()
    {
        // Singleton pattern.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // If there's no audio playing and there are clips in the queue, play the next clip.
        if (!audioSource.isPlaying && audioQueue.Count > 0)
        {
            audioSource.clip = audioQueue.Dequeue();
            audioSource.Play();
        }
    }

    public void QueueAudioClip(AudioClip clip)
    {
        audioQueue.Enqueue(clip);
    }
}
