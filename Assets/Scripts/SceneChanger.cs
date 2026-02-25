using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    private readonly string titleScene = "Title";
    private readonly string gameplayScene = "Gameplay";
    private readonly string endingScene = "Ending";
    private readonly string victoryScene = "Victory";

    private bool isTransitioning;

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
            Button startButton = GameObject.Find("StartButton")?.GetComponent<Button>();
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnRestartGame);
                startButton.onClick.AddListener(OnRestartGame);
            }
        }
    }

    public void OnRestartGame()
    {
        if (isTransitioning) return;

        if (CurrentScene == titleScene)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.DoorOpen);
            StartLoadScene(gameplayScene);
        }
        else if (CurrentScene == endingScene || CurrentScene == victoryScene)
        {
            StartLoadScene(titleScene);
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.DoorClose);
            StartLoadScene(victoryScene);
        }
    }

    public void OnHealthBarDepleted()
    {
        if (isTransitioning) return;

        if (CurrentScene == gameplayScene)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.DefeatSound);
            StartLoadScene(endingScene);
        }
    }

    public void OnVictory()
    {
        if (isTransitioning) return;

        if (CurrentScene == gameplayScene)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Data.DoorClose);
            StartLoadScene(victoryScene);
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

    private void StartLoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isTransitioning = true;

        if (ScreenFader.Instance != null) yield return ScreenFader.Instance.FadeOut();

        SceneManager.LoadScene(sceneName);

        yield return new WaitForFixedUpdate();

        if (ScreenFader.Instance != null) yield return ScreenFader.Instance.FadeIn();

        isTransitioning = false;
    }
}