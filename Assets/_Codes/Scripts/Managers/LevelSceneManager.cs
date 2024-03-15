using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] Material defaultSkybox;
    [SerializeField] Material apocalypticSkybox;
    public static LevelSceneManager Instance;

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
            EnableCursor();
    }

    // Change skybox dependig on levels
    private void Update()
    {
        if (ActiveScene() == "Post Apocalyptic")
            RenderSettings.skybox = apocalypticSkybox;
        else
            RenderSettings.skybox = defaultSkybox;
    }

    public string ActiveScene() => SceneManager.GetActiveScene().name;

    public void GoToScene(string sceneName) => SceneManager.LoadSceneAsync(sceneName);

    public void Play() => GoToScene("Post Apocalyptic"); // change to the scene name of the first level

    public void Quit() => Application.Quit();

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f;
        GoToScene("MainMenu");
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