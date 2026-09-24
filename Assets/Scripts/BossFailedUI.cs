using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasGroup))]
public class BossFailedUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup parentCanvasGroup;
    [SerializeField] private CanvasGroup outOfTimeText;
    [SerializeField] private CanvasGroup moneyMadeText;
    [SerializeField] private CanvasGroup loyaltyPointText;
    [SerializeField] private CanvasGroup comeBackText;
    [SerializeField] private CanvasGroup clickToContinueText;

    [Header("Animation Settings")]
    [SerializeField] private float animTime;
    [SerializeField] private float delayBetweenTexts;
    
    private bool isAnimInDone;

    void Awake()
    {
        parentCanvasGroup = GetComponent<CanvasGroup>();
        parentCanvasGroup.alpha = 0f;
        parentCanvasGroup.interactable = false;
        parentCanvasGroup.blocksRaycasts = false;
    }

    void OnEnable()
    {
        RoundManager.OnBossFailed += Show;
    }

    void OnDisable()
    {
        RoundManager.OnBossFailed -= Show;
    }

    void Show()
    {
        outOfTimeText.alpha = 0f;
        moneyMadeText.alpha = 0f;
        loyaltyPointText.alpha = 0f;
        comeBackText.alpha = 0f;
        clickToContinueText.alpha = 0f;

        parentCanvasGroup.alpha = 1f;
        parentCanvasGroup.interactable = true;
        parentCanvasGroup.blocksRaycasts = true;

        StartCoroutine(AnimateIn());
    }

    private IEnumerator AnimateIn()
    {
        yield return FadeInText(outOfTimeText);
        yield return new WaitForSeconds(delayBetweenTexts);

        yield return FadeInText(moneyMadeText);
        yield return new WaitForSeconds(delayBetweenTexts);
        
        yield return FadeInText(loyaltyPointText);
        yield return new WaitForSeconds(delayBetweenTexts);
        
        yield return FadeInText(comeBackText);
        yield return new WaitForSeconds(delayBetweenTexts);
        
        yield return FadeInText(clickToContinueText);
        isAnimInDone = true;
    }

    private IEnumerator FadeInText(CanvasGroup canvasGroup)
    {
        yield return BasicAnimations.Interpolate(
            () => { canvasGroup.alpha = 0; },
            (t) =>
            {
                canvasGroup.alpha = t;
            },
            () => { canvasGroup.alpha = 1; },
            animTime
        );
    }

    void Update()
    {
        if (isAnimInDone && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // TODO: Load prestige upgrade scene
            TransitionManager.Instance.StartFadeOutIn(SceneType.MainMenu);
        }
    }
}
