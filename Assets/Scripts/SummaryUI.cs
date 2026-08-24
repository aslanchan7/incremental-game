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
        accuracy.text = $"Accuracy: {(float)targetSpawner.TotalTargetsHit / targetSpawner.TotalShotsFired * 100:F0}%";
        targetsShot.text = $"Targets Shot: {targetSpawner.TotalTargetsHit}";
        bullseye.text = $"Bullseye: {(float)targetSpawner.TotalBullseyesHit / targetSpawner.TotalTargetsHit * 100:F0}%";
        timeTaken.text = $"Time Taken: {Time.time - targetSpawner.RoundStartTime:F1}s";
        moneyEarned.text = $"Money Earned: ${targetSpawner.TotalMoneyEarned.Round():F0}";
    }

    public void HandleUpgradesButton()
    {
        SceneManager.LoadScene(1);
    }

    public void HandleContinueButton()
    {
        SceneManager.LoadScene(0);
    }
}
