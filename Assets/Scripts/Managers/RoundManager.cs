using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.UI;

[AutoStaticsCleanup]
public partial class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("References")]
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private BossSpawner bossSpawner;
    [SerializeField] private List<Transform> roundIndicators;

    [Header("Round Info")]
    private bool isBossRound = false;

    [Header("Stats for Summary UI")]
    [HideInInspector] public float Accuracy;
    [HideInInspector] public int TotalTargetsSpawned;
    [HideInInspector] public int TotalTargetsHit;
    [HideInInspector] public int TotalBullseyesHit;
    [HideInInspector] public int TotalShotsFired;
    [HideInInspector] public int TotalShotsMissed;
    [HideInInspector] public float RoundStartTime;
    [HideInInspector] public double TotalMoneyEarned;
    [HideInInspector] public double SpeedBonusCashEarned;
    [HideInInspector] public double AccuracyBonusCashEarned;

    [Header("Actions")]
    public static Action OnRoundStart;
    public static Action OnRoundEnd;

    void OnEnable()
    {
        if (TransitionManager.Instance != null)
            TransitionManager.Instance.OnFadeIn += HandleFadeIn;
    }

    void OnDisable()
    {
        TransitionManager.Instance.OnFadeIn -= HandleFadeIn;
    }

    void Start()
    {
        GameManager.Instance.CurrentRound++;
        isBossRound = GameManager.Instance.CurrentRound % 1 == 0;

        UpdateUI();

        if (isBossRound)
        {
            targetSpawner.gameObject.SetActive(false);
            bossSpawner.gameObject.SetActive(true);
            bossSpawner.StartRound();
        }
        else
        {
            targetSpawner.gameObject.SetActive(true);
            bossSpawner.gameObject.SetActive(false);
            targetSpawner.StartRound();
        }

        OnRoundStart?.Invoke();
        RoundStartTime = Time.time; // This is here in case TransitionManager does not trigger OnFadeIn. If OnFadeIn is triggered then this value will be overwritten.
    }

    void HandleFadeIn()
    {
        RoundStartTime = Time.time;
    }

    public void EndNormalRound(float timePerTarget)
    {
        RoundRuntimeData roundRuntimeData = GameManager.Instance.RoundRuntimeData;

        // Apply Speed Bonus
        float extraTime = (TotalTargetsHit * timePerTarget) - (Time.time - RoundStartTime);
        if (extraTime > 0)
        {
            SpeedBonusCashEarned = (int)(extraTime + 1) * roundRuntimeData.SpeedBonusCash;
            CurrencyManager.Instance.Add("cash", SpeedBonusCashEarned);
        }

        // Apply Accuracy Bonus
        Accuracy = (TotalShotsFired - TotalShotsMissed) / (float)RoundManager.Instance.TotalShotsFired;
        if (Accuracy >= 0.9f)
        {
            AccuracyBonusCashEarned = TotalMoneyEarned * roundRuntimeData.AccuracyBonusCashPercentage;
            CurrencyManager.Instance.Add("cash", AccuracyBonusCashEarned);
        }

        TotalMoneyEarned += SpeedBonusCashEarned + AccuracyBonusCashEarned;

        OnRoundEnd?.Invoke();
    }

    //  -------------- UI STUFF ---------------
    void UpdateUI()
    {
        for (int i = 0; i < roundIndicators.Count; i++)
        {
            if ((i+1) % 1 == 0)
            {
                roundIndicators[i].GetComponent<Image>().color *= Color.red;
            }

            if (i == (GameManager.Instance.CurrentRound % 10) - 1)
            {
                roundIndicators[i].localScale *= 1.25f;
            }
            else if (i < (GameManager.Instance.CurrentRound % 10) - 1)
            {
                roundIndicators[i].GetComponent<Image>().color *= Color.gray;
            }
        }
    }


}
