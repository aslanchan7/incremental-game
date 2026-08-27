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
        UpdateStats();
    }

    void HideSummaryScreen()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void UpdateStats()
    {
        accuracy.text = $"{(float)targetSpawner.TotalTargetsHit / targetSpawner.TotalShotsFired * 100:F0}%";
        targetsShot.text = $"{targetSpawner.TotalTargetsHit}";
        bullseye.text = $"{(float)targetSpawner.TotalBullseyesHit / targetSpawner.TotalTargetsHit * 100:F0}%";
        timeTaken.text = $"{Time.time - targetSpawner.RoundStartTime:F1}s";
        moneyEarned.text = $"Money Earned: ${targetSpawner.TotalMoneyEarned + targetSpawner.SpeedBonusCashEarned:F0}";
        speedMoneyEarned.text = targetSpawner.SpeedBonusCashEarned > 0d ? $"(+${targetSpawner.SpeedBonusCashEarned:F0})" : ""; 
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
