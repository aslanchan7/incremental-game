using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossTimerBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform timerBarParent;
    [SerializeField] private RectTransform timerBarFill;
    [SerializeField] private Slider slider;
    public float RemainingDuration;
    public bool IsAnimating;

    private IEnumerator AnimateTimerBar(float totalDuration)
    {
        float initVal = slider.value;
        yield return BasicAnimations.Interpolate(
            () =>
            {
                IsAnimating = true;
            },
            (t) =>
            {
                slider.value = 1-t;
                RemainingDuration = (1-t) * totalDuration;
            },
            () =>
            {
                IsAnimating = false;
            },
            totalDuration
        );
    }

    public void StartTimerBar(float totalDuration)
    {
        RemainingDuration = totalDuration;
        StartCoroutine(AnimateTimerBar(totalDuration));
    }
}
