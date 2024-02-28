using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script can be found in 'LevelTriggerTransition' prefab.
/// </summary>
public class LevelTransitionTrigger : MonoBehaviour
{
    public enum EventType { OnTriggerEnter }

    EventType triggerEvent = EventType.OnTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && triggerEvent == EventType.OnTriggerEnter)
            LevelTransitionManager.Instance.HandleLevelTransition();
    }
}
