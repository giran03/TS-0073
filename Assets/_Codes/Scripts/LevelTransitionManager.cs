using System;
using UnityEngine;

/// this script handles levels transition <summary>
/// this script can be found under 'GameManager' in the heirarchy
/// 
/// put the an empty game object in the 'levelDefaultSpawn' array for the succeeding levels.
/// the player will teleport to the levelDefaultSpawn empty game object's position.
/// </summary>

public class LevelTransitionManager : MonoBehaviour
{
    [SerializeField] GameObject[] levelDefaultSpawn;
    [SerializeField] GameObject[] levels;

    public static LevelTransitionManager Instance;
    PlayerController playerController;
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Transform parentTransform = playerObj.transform.parent;
        if (parentTransform != null)
        {
            GameObject parentObj = parentTransform.gameObject;
            playerController = parentObj.GetComponent<PlayerController>();
        }
    }

    public void HandleLevelTransition()
    {
        try
        {
            if (currentLevelIndex < levelDefaultSpawn.Length)
            {
                // Get the new position and rotation from the next levelPosition GameObject
                levelDefaultSpawn[currentLevelIndex].transform.GetPositionAndRotation(out Vector3 newPosition, out Quaternion newRotation);

                playerController.TransitionLevel(newPosition, newRotation);

                levelDefaultSpawn[currentLevelIndex].SetActive(false);
                ChangeLevels();
            }
            else
            {
                Debug.Log("No more levels remaining.");
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.LogError("LevelTransitionManager: Index out of range. No more level transitions" + e);
        }
    }

    void ChangeLevels()
    {
        if (currentLevelIndex > levels.Length) return;

        GameObject lastLevel = levels[currentLevelIndex];
        lastLevel.SetActive(!lastLevel.activeSelf);

        currentLevelIndex++;

        GameObject nextLevel = levels[currentLevelIndex];
        nextLevel.SetActive(!nextLevel.activeSelf);
    }
}
