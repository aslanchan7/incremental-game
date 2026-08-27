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
    private RoundRuntimeData roundRuntimeData;

    [Header("Actions")]
    public Action OnTargetsCleared;
    public Action OnRoundStart;
    
    private List<GameObject> spawnedTargets = new();
    private int remainingTargets;

    private int screenSegments;
    private float minX, maxX;
    private float minY, maxY;
    private float segmentWidth;

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

    /// <summary>
    /// Spawns initial targets based on the num of screenSegments. This will spawn 1 target per segement.
    /// It then calls SpawnTargetsOverTime to gradually spawn the rest of the targets.
    /// </summary>
    void SpawnInitialTargets()
    {
        screenSegments = Mathf.Min(roundRuntimeData.TargetCount, maxScreenSegments);

        if (screenSegments <= 0)
        {
            Debug.LogError("Screen Segments is less than or equal to 0");
        }

        minX = spawnPaddingLeft;
        maxX = Screen.width - spawnPaddingRight;
        minY = spawnPaddingBottom;
        maxY = Screen.height - spawnPaddingTop;
        segmentWidth = Screen.width / screenSegments;

        for (int i = 0; i < screenSegments; i++)
        {
            Vector2 worldPos;
            int attempts = 0;
            const int maxAttempts = 30;
            do
            {
                worldPos = GetRandomSpawnWorldPos(i);
                attempts++;             
            } while (!IsPositionClear(worldPos, minDistBetweenTargets, out _) && attempts < maxAttempts);

            if(attempts >= maxAttempts)
            {
                Debug.LogWarning($"Couldn't find a valid spawn position after {maxAttempts} attempts.");
                continue;
            }

            GameObject instantiated = Instantiate(targetPrefab, worldPos, Quaternion.identity, transform);
            spawnedTargets.Add(instantiated);
            TotalTargetsSpawned++;
            remainingTargets++;
        }

        StartCoroutine(SpawnTargetsOverTime());
    }

    IEnumerator SpawnTargetsOverTime()
    {
        while (TotalTargetsSpawned < roundRuntimeData.TargetCount)
        {
            yield return new WaitForSeconds(roundRuntimeData.TimeBetweenSpawns);   
            int segmentIdx = Random.Range(0, screenSegments);
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
                yield return null;
            }
            
            GameObject instantiated = Instantiate(targetPrefab, worldPos, Quaternion.identity, transform);
            spawnedTargets.Add(instantiated);
            TotalTargetsSpawned++;
            remainingTargets++;
        }
    }

    public bool IsPositionClear(Vector2 worldPos, float radius, out Collider2D[] hits) {
        hits = Physics2D.OverlapCircleAll(worldPos, radius, targetLayer);
        return hits.Length == 0;
    }

    void DestroyTargets()
    {
        StopCoroutine(SpawnTargetsOverTime());
        foreach (var target in spawnedTargets)
        {
            Destroy(target);
        }
        TotalTargetsSpawned = 0;
        remainingTargets = 0;
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
            spawnedTargets.Remove(target);
            remainingTargets--;
            TotalTargetsHit++;
            
            BigDouble moneyEarned = isBullseye ? roundRuntimeData.BaseTargetValue * roundRuntimeData.BullseyeMultiplier : roundRuntimeData.BaseTargetValue;
            TotalMoneyEarned += moneyEarned;
            TotalBullseyesHit += isBullseye ? 1 : 0;

            CurrencyManager.Instance.Add("cash", moneyEarned);
        }
        
        if (remainingTargets == 0)
        {
            // Apply Speed Bonus
            float extraTime = (TotalTargetsHit * timePerTarget) - (Time.time - RoundStartTime);
            if (extraTime > 0)
            {
                SpeedBonusCashEarned = (int)extraTime * roundRuntimeData.SpeedBonusCash;
                CurrencyManager.Instance.Add("cash", SpeedBonusCashEarned);
            }
            
            OnTargetsCleared?.Invoke();
            spawnedTargets.Clear();
        }
    }
}
