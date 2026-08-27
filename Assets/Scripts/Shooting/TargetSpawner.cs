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
    [SerializeField] private float timePerTarget = 0.5f;
    [SerializeField] private LayerMask targetLayer;

    [Header("References")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private ShootingModule shootingModule;
    [HideInInspector] public List<GameObject> SpawnedTargets = new();
    private RoundRuntimeData roundRuntimeData;

    [Header("Actions")]
    public Action OnTargetsCleared;
    public Action OnRoundStart;

    [Header("Chance Bags")]
    [SerializeField] private ChanceBag targetRespawnChanceBag;
    
    private int remainingTargets;

    private int screenSegments;
    private float minX, maxX;
    private float minY, maxY;
    private float segmentWidth;
    private bool isSpawningTargetsOverTime;

    [Header("Stats for Summary UI")]
    [HideInInspector] public int TotalTargetsSpawned;
    [HideInInspector] public int TotalTargetsHit;
    [HideInInspector] public int TotalBullseyesHit;
    [HideInInspector] public int TotalShotsFired;
    [HideInInspector] public float RoundStartTime;
    [HideInInspector] public BigDouble TotalMoneyEarned;
    [HideInInspector] public double SpeedBonusCashEarned;

    public bool refresh;

    void Start()
    {
        roundRuntimeData = GameManager.Instance.RoundRuntimeData;
        SpawnInitialTargets();
        RoundStartTime = Time.time;
        OnRoundStart?.Invoke();
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
        shootingModule.ShotFired += HandleShotFired;
    }

    void OnDisable()
    {
        shootingModule.ShotFired -= HandleShotFired;
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
        screenSegments = Mathf.Min(roundRuntimeData.InitialTargetCount, maxScreenSegments);

        if (screenSegments <= 0)
        {
            Debug.LogError("Screen Segments is less than or equal to 0");
        }

        minX = spawnPaddingLeft;
        maxX = Screen.width - spawnPaddingRight;
        minY = spawnPaddingBottom;
        maxY = Screen.height - spawnPaddingTop;
        segmentWidth = Screen.width / screenSegments;

        for (int i = 0; i < roundRuntimeData.InitialTargetCount; i++)
        {
            SpawnTarget(i % screenSegments);
        }

        StartCoroutine(SpawnTargetsOverTime());
    }

    IEnumerator SpawnTargetsOverTime()
    {
        isSpawningTargetsOverTime = true;
        while (TotalTargetsSpawned < roundRuntimeData.TotalTargetCount)
        {
            yield return new WaitForSeconds(roundRuntimeData.TimeBetweenSpawns);   
            int segmentIdx = Random.Range(0, screenSegments);
            SpawnTarget(segmentIdx);
        }
        isSpawningTargetsOverTime = false;
    }

    public bool IsPositionClear(Vector2 worldPos, float radius, out Collider2D[] hits) {
        hits = Physics2D.OverlapCircleAll(worldPos, radius, targetLayer);
        return hits.Length == 0;
    }

    void DestroyTargets()
    {
        StopCoroutine(SpawnTargetsOverTime());
        foreach (var target in SpawnedTargets)
        {
            Destroy(target);
        }
        TotalTargetsSpawned = 0;
        remainingTargets = 0;
    }

    private void SpawnTarget(int segmentIdx)
    {
        Vector2 worldPos;
        int attempts = 0;
        const int maxAttempts = 30;
        do
        {
            worldPos = GetRandomSpawnWorldPos(segmentIdx);
            attempts++;             
        } while (!IsPositionClear(worldPos, minDistBetweenTargets, out _) && attempts < maxAttempts);

        if(attempts >= maxAttempts)
        {
            Debug.LogWarning($"Couldn't find a valid spawn position after {maxAttempts} attempts.");
            return;
        }
        
        GameObject instantiated = Instantiate(targetPrefab, worldPos, Quaternion.identity, transform);
        SpawnedTargets.Add(instantiated);
        TotalTargetsSpawned++;
        remainingTargets++;
    }

    Vector2 GetRandomSpawnWorldPos(int segmentIndex)
    {
        float randX = Random.Range(0f, 1f);
        float xPos = (segmentWidth * segmentIndex) + (segmentWidth * randX);
        xPos = xPos < minX 
            ? minX 
            : xPos > maxX 
                ? maxX 
                : xPos;

        float randY = Random.Range(0f, 1f);
        float yPos = randY * Screen.height;
        yPos = yPos < minY 
            ? minY 
            : yPos > maxY 
                ? maxY 
                : yPos;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(new(xPos, yPos));

        return worldPos;
    }

    void HandleShotFired(GameObject target, bool isBullseye)
    {
        TotalShotsFired++;

        if (target != null)
        {
            SpawnedTargets.Remove(target);
            remainingTargets--;
            TotalTargetsHit++;
            
            BigDouble moneyEarned = isBullseye ? roundRuntimeData.BaseTargetValue * roundRuntimeData.BullseyeMultiplier : roundRuntimeData.BaseTargetValue;
            TotalMoneyEarned += moneyEarned;
            TotalBullseyesHit += isBullseye ? 1 : 0;

            CurrencyManager.Instance.Add("cash", moneyEarned);

            // Potentially respawn target here based on TargetRespawnChance
            if (roundRuntimeData.TargetRespawnChance != 0f)
            {
                if (targetRespawnChanceBag.IsEmpty)
                    targetRespawnChanceBag.NewBag(roundRuntimeData.TargetRespawnChance);

                bool respawnTarget = targetRespawnChanceBag.Pull();
                if (respawnTarget)
                {
                    int segmentIdx = Random.Range(0, screenSegments);
                    SpawnTarget(segmentIdx);
                }                
            }

        }
        
        if (remainingTargets == 0 && !isSpawningTargetsOverTime)
        {
            // Apply Speed Bonus
            float extraTime = (TotalTargetsHit * timePerTarget) - (Time.time - RoundStartTime);
            if (extraTime > 0)
            {
                SpeedBonusCashEarned = (int)extraTime * roundRuntimeData.SpeedBonusCash;
                CurrencyManager.Instance.Add("cash", SpeedBonusCashEarned);
            }
            
            OnTargetsCleared?.Invoke();
            SpawnedTargets.Clear();
        }
    }
}
