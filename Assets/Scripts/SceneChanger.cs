using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    private readonly string titleScene = "Title";
    private readonly string gameplayScene = "Gameplay";
    private readonly string endingScene = "Ending";

    public string CurrentScene => SceneManager.GetActiveScene().name;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == titleScene)
        {
            GameObject.Find("Button").GetComponent<Button>().onClick.AddListener(OnRestartGame);
        }
    }

    public void OnRestartGame()
    {
        if (CurrentScene == titleScene)
        {
            SceneManager.LoadScene(gameplayScene);
        }
        else
        {
            SceneManager.LoadScene(titleScene);
        }
    }

    public void OnHealthBarDepleted()
    {
        if (CurrentScene == gameplayScene)
        {
            SceneManager.LoadScene(endingScene);
        }
    }

    private void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}