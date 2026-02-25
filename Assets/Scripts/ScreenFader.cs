using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    public float FadeDuration = 0.5f;

    private Canvas canvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!canvas) canvas = GetComponent<Canvas>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        AttachToMainCamera();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachToMainCamera();
    }

    void AttachToMainCamera()
    {
        Camera cam = Camera.main;

        if (!cam || !canvas)
            return;

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = cam;
        }
    }

    public IEnumerator FadeOut() => Fade(1f);
    public IEnumerator FadeIn() => Fade(0f);

    private IEnumerator Fade(float target)
    {
        canvasGroup.blocksRaycasts = true;

        float start = canvasGroup.alpha;
        float time = 0f;

        while (time < FadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / FadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
        canvasGroup.blocksRaycasts = target > 0.001f;
    }
}