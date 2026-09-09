using System;
using System.Collections;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public partial class TransitionManager : MonoBehaviour
{
    [AutoStaticsCleanup] public static TransitionManager Instance = null;

    [Header("References")]
    private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    private float fadeInAnimTime = 0.5f;
    private float fadeOutAnimTime = 0.5f;

    [Header("Actions")]
    public Action OnFadeIn;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartFadeOutIn(int sceneIndex)
    {
        StartCoroutine(FadeOutIn(sceneIndex));
    }

    private IEnumerator FadeOutIn(int sceneIndex)
    {
        yield return FadeOut(sceneIndex);
        yield return FadeIn();
    }

    private IEnumerator FadeOut(int sceneIndex)
    {
        yield return BasicAnimations.Interpolate(
            () =>
            {
                canvasGroup.alpha = 0f;
            },
            (t) =>
            {
                canvasGroup.alpha = t;
            },
            () =>
            {
                SceneManager.LoadScene(sceneIndex);        
            },
            fadeOutAnimTime
        );
    }

    private IEnumerator FadeIn()
    {
        yield return BasicAnimations.Interpolate(
            () =>
            {
                canvasGroup.alpha = 1f;
            },
            (t) =>
            {
                canvasGroup.alpha = 1-t;
            },
            () =>
            {
                OnFadeIn?.Invoke();
            },
            fadeInAnimTime
        );
    }
}
