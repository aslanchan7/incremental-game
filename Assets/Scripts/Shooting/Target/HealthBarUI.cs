using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform healthBarParent;
    [SerializeField] private RectTransform healthBarFill;
    [SerializeField] private Slider slider;
    public bool IsAnimating;
    private const float DEFAULT_ANIM_TIME = 0.1f;

    public void SetPositionWorldSpace(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        screenPos -= new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        healthBarParent.localPosition = screenPos;
    }

    public IEnumerator UpdateHealthBar(float currHealth, float maxHealth, float healthBarAnimTime = DEFAULT_ANIM_TIME)
    {
        float initVal = slider.value;
        yield return BasicAnimations.Interpolate(
            () =>
            {
                IsAnimating = true;
            },
            (t) =>
            {
                t = BasicAnimations.EaseOut(t);
                slider.value = Mathf.Lerp(initVal, currHealth/maxHealth, t);
            },
            () =>
            {
                IsAnimating = false;
            },
            healthBarAnimTime
        );
    }
}