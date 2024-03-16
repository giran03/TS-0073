using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] Material defaultSkybox;
    [SerializeField] Material apocalypticSkybox;
    public static LevelSceneManager Instance;
    string activeScene;

    // MAIN MENU
    [Header("Main Menu Settings")]
    [SerializeField] GameObject[] mainMenuEnvironments = null; // Array of game objects to cycle
    int currentIndex = 0; // Index of the currently active object

    private void Awake()
    {
        // singleton pattern
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        QualitySettings.vSyncCount = 1;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            EnableCursor();
            StartCoroutine(MainMenuAnimations());
        }
        if (SceneManager.GetActiveScene().name == "EndScreen")
            EnableCursor();
    }

    // Change skybox dependig on levels
    private void Update()
    {
        activeScene = ActiveScene();

        // SKYBOX
        if (ActiveScene() == "Post Apocalyptic")
            RenderSettings.skybox = apocalypticSkybox;
        else
            RenderSettings.skybox = defaultSkybox;
    }

    public string ActiveScene() => SceneManager.GetActiveScene().name;

    public void GoToScene(string sceneName) => SceneManager.LoadSceneAsync(sceneName);

    public void Play() => GoToScene("001_Tutorial"); // change to the scene name of the first level

    public void Quit() => Application.Quit();

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f;
        GoToScene("MainMenu");
    }

    public void LevelFinish()   // FIXME: RE ORDER LEVELS
    {
        if (activeScene == "001_Tutorial")
            GoToScene("002_LV1");
        else if (activeScene == "002_LV1")
            GoToScene("003_LV2");
        else if (activeScene == "003_LV2")
            GoToScene("Level");
        else if (activeScene == "Level")
            GoToScene("Post Apocalyptic");
        else if (activeScene == "Post Apocalyptic")
            GoToScene("EndScreen");
        else
            EndScreen();
    }

    public void EndScreen() => GoToScene("EndScreen");

    public void PauseGame(GameObject pauseMenu)
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        EnableCursor();
    }

    public void ResetCurrentLevel()
    {
        Time.timeScale = 1f;
        GoToScene(ActiveScene());
        DisableCursor();
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

    IEnumerator MainMenuAnimations()
    {
        if (mainMenuEnvironments != null)
            while (true)
            {
                // Disable the current active object
                mainMenuEnvironments[currentIndex].SetActive(false);
                // Move to the next object in the array
                currentIndex = (currentIndex + 1) % mainMenuEnvironments.Length;
                // Enable the next object
                mainMenuEnvironments[currentIndex].SetActive(true);
                yield return new WaitForSeconds(3f);
            }

    }
}