using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    public static LevelSceneManager Instance;
    public static Action<string> OnSceneChanged;

    private void Awake()
    {
        // singleton pattern
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        OnSceneChanged += (scene) => GoToScene(scene);

        QualitySettings.vSyncCount = 1;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            EnableCursor();
    }

    public void GoToScene(string sceneName) => SceneManager.LoadSceneAsync(sceneName);

    public void Play() => OnSceneChanged("Template_Level"); // change to the scene name of the first level

    public void Quit() => Application.Quit();

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f;
        OnSceneChanged("MainMenu");
    }


    public void PauseGame(GameObject pauseMenu)
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        EnableCursor();
    }

    public void ResumeGame(GameObject pauseMenu)
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        DisableCursor();
    }

    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
