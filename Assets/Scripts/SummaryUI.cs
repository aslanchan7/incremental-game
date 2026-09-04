using System.Collections;
using BreakInfinity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] TextMeshProUGUI accuracy;
    [SerializeField] TextMeshProUGUI targetsShot;
    [SerializeField] TextMeshProUGUI bullseye;
    [SerializeField] TextMeshProUGUI timeTaken;
    [SerializeField] TextMeshProUGUI moneyEarned;
    [SerializeField] TextMeshProUGUI speedMoneyEarned;
    [SerializeField] TextMeshProUGUI accuracyMoneyEarned;

    [Header("Animation Settings")]
    [SerializeField] private float animTime = 1f;

    void Awake()
    {
        HideSummaryScreen();
    }

    void OnEnable()
    {
        targetSpawner.OnTargetsCleared += ShowSummaryScreen;
    }

    void OnDisable()
    {
        targetSpawner.OnTargetsCleared -= ShowSummaryScreen;
    }

    void ShowSummaryScreen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(UpdateStats());
    }

    void HideSummaryScreen()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private IEnumerator UpdateStats()
    {
        // accuracy.text = $"{targetSpawner.Accuracy * 100:F0}%";
        // targetsShot.text = $"{targetSpawner.TotalTargetsHit}";
        // bullseye.text = $"{(float)targetSpawner.TotalBullseyesHit / targetSpawner.TotalTargetsHit * 100:F0}%";
        // timeTaken.text = $"{Time.time - targetSpawner.RoundStartTime:F1}s";
        // moneyEarned.text = $"Money Earned: ${targetSpawner.TotalMoneyEarned.ToDouble():F0}";
        // accuracyMoneyEarned.text = targetSpawner.AccuracyBonusCashEarned > 0d ? $"(+${targetSpawner.AccuracyBonusCashEarned:F0})" : "";

        // Initialize string values
        accuracy.text = "0%";
        targetsShot.text = "0";
        bullseye.text = "0%";
        timeTaken.text = "0s";
        moneyEarned.text = "Money Earned: $0";
        speedMoneyEarned.text = "";
        accuracyMoneyEarned.text = "";

        StartCoroutine(IncrementText(accuracy, 0f, targetSpawner.Accuracy * 100f, animTime, "{0:F0}%"));
        StartCoroutine(IncrementText(targetsShot, 0f, targetSpawner.TotalTargetsHit, animTime, "{0:F0}"));
        StartCoroutine(IncrementText(bullseye, 0f, (float)targetSpawner.TotalBullseyesHit / targetSpawner.TotalTargetsHit * 100, animTime, "{0:F0}%"));
        StartCoroutine(IncrementText(timeTaken, 0f, Time.time - targetSpawner.RoundStartTime, animTime, "{0:F1}s"));

        if (targetSpawner.AccuracyBonusCashEarned > 0d)
            StartCoroutine(IncrementText(accuracyMoneyEarned, 0f, (float)targetSpawner.AccuracyBonusCashEarned, animTime, "(+${0:F0})"));
        else
            accuracyMoneyEarned.text = "";

        if (targetSpawner.SpeedBonusCashEarned > 0d)
            StartCoroutine(IncrementText(speedMoneyEarned, 0f, (float)targetSpawner.SpeedBonusCashEarned, animTime, "(+${0:F0})"));
        else
            speedMoneyEarned.text = "";


        yield return new WaitForSeconds(animTime);
        StartCoroutine(IncrementText(moneyEarned, 0f, (float)targetSpawner.TotalMoneyEarned.ToDouble(), animTime, "Money Earned: ${0:F0}"));
    }

    private IEnumerator IncrementText(TextMeshProUGUI tmp, float startNum, float endNum, float animTime, string format = "{0:F0}")
    {
        yield return BasicAnimations.Interpolate(
            () =>
            {
                tmp.text = $"{startNum:F0}";
            },
            (t) =>
            {
                float tween = BasicAnimations.EaseOutExpo(t);
                float num = Mathf.Lerp(startNum, endNum, tween);
                tmp.text = string.Format(format, num);
            },
            () =>
            {
                tmp.text = string.Format(format, endNum);                
            },
            animTime
        );
    }

    public void HandleUpgradesButton()
    {
        TransitionManager.Instance.StartFadeOutIn(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void HandleContinueButton()
    {
        TransitionManager.Instance.StartFadeOutIn(SceneManager.GetActiveScene().buildIndex);
    }
}
