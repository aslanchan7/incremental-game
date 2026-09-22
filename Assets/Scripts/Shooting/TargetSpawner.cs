using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class TargetSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxScreenSegments; // how many segments to split the screen into for spawning targets evenly
    [Space(10)]
    // [SerializeField, Range(0, 1)] private float spawnPaddingTop;
    // [SerializeField, Range(0, 1)] private float spawnPaddingBottom;
    // [SerializeField, Range(0, 1)] private float spawnPaddingLeft;
    // [SerializeField, Range(0, 1)] private float spawnPaddingRight;
    [SerializeField] private BoxCollider2D spawnBounds;
    [Space(10)]
    [SerializeField] private float minDistBetweenTargets = 1f;
    [SerializeField] float timePerTarget = 1f;
    public LayerMask TargetLayer;

    [Header("References")]
    [SerializeField] private DefaultTarget defaultTargetPrefab;
    public GoldenTarget GoldenTargetPrefab;
    [HideInInspector] public List<Target> SpawnedTargets = new();
    private RoundRuntimeData roundRuntimeData;

    [Header("Golden Target Settings")]
    private List<GoldenTarget> goldenTargetList = new();

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

    [Header("Combo Settings")]
    public float comboMultPerHit = 0.1f;
    public int CurrCombo;

    [Header("Target Variables")]
    [HideInInspector] public int RemainingTargets;

    [Header("Debug")]
    [SerializeField] private bool refresh;

    public void StartRound()
    {
        roundRuntimeData = GameManager.Instance.RoundRuntimeData;
        SpawnInitialTargets();
    }

    void Update()
    {
        if (refresh || Keyboard.current.rKey.wasPressedThisFrame)
        {
            refresh = false;
            Refresh();
        }

        if (DebugManager.Instance.AllowSkipRounds)
        {
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                Target[] targets = new Target[SpawnedTargets.Count];
                SpawnedTargets.CopyTo(targets);
                foreach (var target in targets)
                {
                    HandleShotFired(target, true, true, false, Vector3.zero);
                }
            }
        }
    }

    void OnEnable()
    {
        // TODO: MAYBE REFACTOR? IDK IF TARGET SPAWNER NEEDS TO HAVE A REFERENCE TO SHOOTING MODULE
        // shootingModule.OnGunInitialized += HandleGunInitialized;
        Gun.ShotFired += HandleShotFired;
    }

    void OnDisable()
    {
        // shootingModule.ShotFired -= HandleShotFired;
        // shootingModule.OnGunInitialized -= HandleGunInitialized;
        Gun.ShotFired -= HandleShotFired;
    }

    // private void HandleGunInitialized()
    // {
    //     Gun.ShotFired += HandleShotFired;
    // }

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

        SegmentWidth = spawnBounds.bounds.size.x / ScreenSegments;

        for (int i = 0; i < roundRuntimeData.InitialTargetCount; i++)
        {
            SpawnTarget(i % ScreenSegments, defaultTargetPrefab);
        }
    }

    void DestroyTargets()
    {
        foreach (var target in SpawnedTargets)
        {
            Destroy(target.gameObject);
        }
        SpawnedTargets.Clear();
        RoundManager.Instance.TotalTargetsSpawned = 0;
        RemainingTargets = 0;
    }

    public void SpawnTarget(int segmentIdx, Target targetPrefab)
    {
        float radius = targetPrefab.GetComponent<CircleCollider2D>().radius;
        MinX = spawnBounds.bounds.min.x + radius;
        MaxX = spawnBounds.bounds.max.x - radius;
        MinY = spawnBounds.bounds.min.y + radius;
        MaxY = spawnBounds.bounds.max.y - radius;

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

        Target instantiated = Instantiate(targetPrefab, worldPos, Quaternion.identity, transform);
        instantiated.TargetSpawner = this;

        SpawnedTargets.Add(instantiated);
        RoundManager.Instance.TotalTargetsSpawned++;
        RemainingTargets++;
    }

    Vector2 GetRandomSpawnWorldPos(int segmentIndex)
    {
        float randX = Random.Range(0f, 1f);
        float xPos = spawnBounds.bounds.min.x + (SegmentWidth * segmentIndex) + (SegmentWidth * randX);
        xPos = xPos < MinX
            ? MinX
            : xPos > MaxX
                ? MaxX
                : xPos;

        // float randY = Random.Range(0f, 1f);
        // float yPos = randY * spawnBounds.bounds.size.y;
        // yPos = yPos < MinY
        //     ? MinY
        //     : yPos > MaxY
        //         ? MaxY
        //         : yPos;
        float yPos = Random.Range(MinY, MaxY);

        // Vector2 worldPos = Camera.main.ScreenToWorldPoint(new(xPos, yPos));

        // float xPos = Random.Range(MinX, MaxX);
        Vector2 worldPos = new(xPos, yPos);
        return worldPos;
    }

    public bool IsPositionClear(Vector2 worldPos, float radius, out Collider2D[] hits)
    {
        hits = Physics2D.OverlapCircleAll(worldPos, radius, TargetLayer);
        return hits.Length == 0;
    }

    void HandleShotFired(Target target, bool isBullseye, bool isCrit, bool isAerialStrike, Vector3 shotPos)
    {
        RoundManager.Instance.TotalShotsFired++;
        RoundManager.Instance.TotalShotsMissed += target == null ? 1 : 0;
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
            RoundManager.Instance.EndNormalRound(timePerTarget);
            SpawnedTargets.Clear();
        }
    }
}
