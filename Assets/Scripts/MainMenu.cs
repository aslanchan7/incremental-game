using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform title;
    [SerializeField] private RectTransform startButton;
    [SerializeField] private RectTransform quitButton;
    [SerializeField] private CanvasGroup fadeOutBg;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInAnimTime = 1.0f;
    private Vector3 titleTargetPos;
    private Vector3 startBtnTargetPos;
    private Vector3 quitBtnTargetPos;

    [Header("Debug")]
    [SerializeField] private bool fadeIn;
    [SerializeField] private bool fadeOut;

    void Awake()
    {
        titleTargetPos = title.localPosition;
        startBtnTargetPos = startButton.localPosition;
        quitBtnTargetPos = quitButton.localPosition;
    }

    void Start()
    {
        StartCoroutine(FadeInMainMenu());
    }

    void Update()
    {
        if (fadeIn)
        {
            fadeIn = false;
            StartCoroutine(FadeInMainMenu());
        }
    }

    private IEnumerator FadeInMainMenu()
    {
        Vector2 titleInitPos = new(title.localPosition.x, (Screen.currentResolution.height + title.sizeDelta.y) / 2f); 
        Vector2 startBtnInitPos = new(startButton.localPosition.x, -(Screen.currentResolution.height + startButton.sizeDelta.y) / 2f);
        Vector2 quitBtnInitPos = new(quitButton.localPosition.x, -(Screen.currentResolution.height + quitButton.sizeDelta.y) / 2f);

        yield return BasicAnimations.Interpolate(
            () =>
            {
                // Set elements off screen
                title.localPosition = titleInitPos;
                startButton.localPosition = startBtnInitPos;
                quitButton.localPosition = quitBtnInitPos;
            },
            (t) =>
            {
                float tween = BasicAnimations.Quadratic(t);
                title.localPosition = Vector2.Lerp(titleInitPos, titleTargetPos, tween);
                startButton.localPosition = Vector2.Lerp(startBtnInitPos, startBtnTargetPos, tween);
                quitButton.localPosition = Vector2.Lerp(quitBtnInitPos, quitBtnTargetPos, tween);
            },
            null,
            fadeInAnimTime
        );   
    }

    public void StartGame()
    {
        TransitionManager.Instance.StartFadeOutIn(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
