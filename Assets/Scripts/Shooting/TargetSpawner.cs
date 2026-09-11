using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class TargetSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxScreenSegments; // how many segments to split the screen into for spawning targets evenly
    [Space(10)]
    [SerializeField] private float spawnPaddingTop;
    [SerializeField] private float spawnPaddingBottom;
    [SerializeField] private float spawnPaddingLeft;
    [SerializeField] private float spawnPaddingRight;
    [Space(10)]
    [SerializeField] private float minDistBetweenTargets = 1f;
    [SerializeField] float timePerTarget = 1f;
    public LayerMask TargetLayer;

    [Header("References")]
    [SerializeField] private DefaultTarget defaultTargetPrefab;
    [SerializeField] private GoldenTarget goldenTargetPrefab;
    [SerializeField] private ShootingModule shootingModule;
    [HideInInspector] public List<Target> SpawnedTargets = new();
    private RoundRuntimeData roundRuntimeData;

    [Header("Golden Target Settings")]

    private List<GoldenTarget> goldenTargetList = new();

    [Header("Actions")]
    public Action OnTargetsCleared;
    public Action OnRoundStart;

    [Header("Chance Bags")]
    public ChanceBag TargetRespawnChanceBag;
    public ChanceBag GoldenTargetChanceBag;

    [Header("Screen Variables")]
    [HideInInspector] public int ScreenSegments;
    [HideInInspector] public float MinX;
    [HideInInspector] public float MaxX;
    [HideInInspector] public float MinY;
    [HideInInspector] public float MaxY;
    [HideInInspector] public float SegmentWidth;

    [Header("Stats for Summary UI")]
    [HideInInspector] public float Accuracy;
    [HideInInspector] public int TotalTargetsSpawned;
    [HideInInspector] public int TotalTargetsHit;
    [HideInInspector] public int TotalBullseyesHit;
    [HideInInspector] public int TotalShotsFired;
    [HideInInspector] public int TotalShotsMissed;
    [HideInInspector] public float RoundStartTime;
    [HideInInspector] public BigDouble TotalMoneyEarned;
    [HideInInspector] public double SpeedBonusCashEarned;
    [HideInInspector] public double AccuracyBonusCashEarned;
    
    [Header("Combo Settings")]
    public float comboMultPerHit = 0.1f;
    public int CurrCombo;

    [Header("Target Variables")]
    [HideInInspector] public int RemainingTargets;

    [Header("Debug")]
    [SerializeField] private bool refresh;

    void Start()
    {
        roundRuntimeData = GameManager.Instance.RoundRuntimeData;
        SpawnInitialTargets();
        OnRoundStart?.Invoke();
        RoundStartTime = Time.time; // This is here in case TransitionManager does not trigger OnFadeIn. If OnFadeIn is triggered then this value will be overwritten.
    }

    void Update()
    {
        if (refresh)
        {
            refresh = false;
            Refresh();
        }
    }

    void OnEnable()
    {
        // TODO: MAYBE REFACTOR? IDK IF TARGET SPAWNER NEEDS TO HAVE A REFERENCE TO SHOOTING MODULE
        // shootingModule.OnGunInitialized += HandleGunInitialized;
        Gun.ShotFired += HandleShotFired;
        if (TransitionManager.Instance != null)
            TransitionManager.Instance.OnFadeIn += HandleFadeIn;
    }

    void OnDisable()
    {
        // shootingModule.ShotFired -= HandleShotFired;
        // shootingModule.OnGunInitialized -= HandleGunInitialized;
        Gun.ShotFired -= HandleShotFired;
        TransitionManager.Instance.OnFadeIn -= HandleFadeIn;
    }

    private void HandleGunInitialized()
    {
        Gun.ShotFired += HandleShotFired;
    }

    void HandleFadeIn()
    {
        RoundStartTime = Time.time;
    }

    // THIS IS JUST A HELPER FUNCTION, I NEED TO REMOVE THIS
    void Refresh()
    {
        DestroyTargets();
        SpawnInitialTargets();
        // shootingModule.CurrAmmo = shootingModule.MaxAmmo;
    }

    void SpawnInitialTargets()
    {
        ScreenSegments = Mathf.Min(roundRuntimeData.InitialTargetCount, maxScreenSegments);

        if (ScreenSegments <= 0)
        {
            Debug.LogError("Screen Segments is less than or equal to 0");
        }

        MinX = spawnPaddingLeft;
        MaxX = Screen.width - spawnPaddingRight;
        MinY = spawnPaddingBottom;
        MaxY = Screen.height - spawnPaddingTop;
        SegmentWidth = Screen.width / ScreenSegments;

        for (int i = 0; i < roundRuntimeData.InitialTargetCount; i++)
        {
            SpawnTarget(i % ScreenSegments);
        }
    }

    void DestroyTargets()
    {
        foreach (var target in SpawnedTargets)
        {
            Destroy(target);
        }
        TotalTargetsSpawned = 0;
        RemainingTargets = 0;
    }

    public void SpawnTarget(int segmentIdx, bool isGolden = false)
    {
        Vector2 worldPos;
        int attempts = 0;
        const int maxAttempts = 30;
        do
        {
            worldPos = GetRandomSpawnWorldPos(segmentIdx);
            attempts++;
        } while (!IsPositionClear(worldPos, minDistBetweenTargets, out _) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning($"Couldn't find a valid spawn position after {maxAttempts} attempts.");
            return;
        }

        Target prefabToInstantiate = isGolden ? goldenTargetPrefab : defaultTargetPrefab;
        Target instantiated = Instantiate(prefabToInstantiate, worldPos, Quaternion.identity, transform);
        instantiated.TargetSpawner = this;

        if (isGolden)
            goldenTargetList.Add(instantiated.GetComponent<GoldenTarget>());

        SpawnedTargets.Add(instantiated);
        TotalTargetsSpawned++;
        RemainingTargets++;
    }

    Vector2 GetRandomSpawnWorldPos(int segmentIndex)
    {
        float randX = Random.Range(0f, 1f);
        float xPos = (SegmentWidth * segmentIndex) + (SegmentWidth * randX);
        xPos = xPos < MinX
            ? MinX
            : xPos > MaxX
                ? MaxX
                : xPos;

        float randY = Random.Range(0f, 1f);
        float yPos = randY * Screen.height;
        yPos = yPos < MinY
            ? MinY
            : yPos > MaxY
                ? MaxY
                : yPos;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(new(xPos, yPos));

        return worldPos;
    }

    public bool IsPositionClear(Vector2 worldPos, float radius, out Collider2D[] hits)
    {
        hits = Physics2D.OverlapCircleAll(worldPos, radius, TargetLayer);
        return hits.Length == 0;
    }

    void HandleShotFired(Target target, bool isBullseye, bool isCrit, bool isAerialStrike, Vector3 shotPos)
    {
        TotalShotsFired++;
        TotalShotsMissed += target == null ? 1 : 0;
        CurrCombo += target != null ? 1 : 0;

        if (target != null)
        {
            target.HandleShot(isBullseye, isCrit, isAerialStrike, shotPos);
        }

        CheckRoundEndCondition();
    }

    void CheckRoundEndCondition()
    {
        if (RemainingTargets == 0)
        {
            // Apply Speed Bonus
            float extraTime = (TotalTargetsHit * timePerTarget) - (Time.time - RoundStartTime);
            if (extraTime > 0)
            {
                SpeedBonusCashEarned = (int)(extraTime + 1) * roundRuntimeData.SpeedBonusCash;
                CurrencyManager.Instance.Add("cash", SpeedBonusCashEarned);
            }

            // Apply Accuracy Bonus
            Accuracy = (TotalShotsFired - TotalShotsMissed) / (float)TotalShotsFired;
            if (Accuracy >= 0.9f)
            {
                AccuracyBonusCashEarned = TotalMoneyEarned.ToDouble() * roundRuntimeData.AccuracyBonusCashPercentage;
                CurrencyManager.Instance.Add("cash", AccuracyBonusCashEarned);
            }

            TotalMoneyEarned += SpeedBonusCashEarned + AccuracyBonusCashEarned;

            OnTargetsCleared?.Invoke();
            SpawnedTargets.Clear();
        }
    }
}
