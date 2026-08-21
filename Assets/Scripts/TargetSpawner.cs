using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int targetCount; // number of targets we WANT to spawn in total
    [SerializeField] private int maxScreenSegments; // how many segments to split the screen into for spawning targets evenly
    [SerializeField] private float spawnPadding;
    [SerializeField] private float timeBetweenSpawns; // after initial spawning, targets will spawn with this amount of time delay

    [Header("References")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private ShootingModule shootingModule;

    private PlayerControls controls;
    
    private List<GameObject> spawnedTargets = new();
    private int totalTargetsSpawned;

    private int screenSegments;
    private float minX, maxX;
    private float minY, maxY;
    private float segmentWidth;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void Start()
    {
        SpawnInitialTargets();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Refresh.performed += Refresh;

        shootingModule.ShotFired += TryRemoveNullTargets;
    }

    void OnDisable()
    {
        controls.Player.Disable();
        controls.Player.Refresh.performed -= Refresh;

        shootingModule.ShotFired += TryRemoveNullTargets;
    }

    // THIS IS JUST A HELPER FUNCTION, I NEED TO REMOVE THIS
    void Refresh(InputAction.CallbackContext context)
    {
        DestroyTargets();
        SpawnInitialTargets();
        shootingModule.CurrAmmo = shootingModule.MaxAmmo;
    }

    /// <summary>
    /// Spawns initial targets based on the num of screenSegments. This will spawn 1 target per segement.
    /// It then calls SpawnTargetsOverTime to gradually spawn the rest of the targets.
    /// </summary>
    void SpawnInitialTargets()
    {
        screenSegments = Mathf.Min(targetCount, maxScreenSegments);

        if (screenSegments <= 0)
        {
            Debug.LogError("Screen Segments is less than or equal to 0");
        }

        minX = spawnPadding;
        maxX = Screen.width - spawnPadding;
        minY = spawnPadding;
        maxY = Screen.height - spawnPadding;
        segmentWidth = Screen.width / screenSegments;

        for (int i = 0; i < screenSegments; i++)
        {
            Vector2 worldPos = GetRandomSpawnWorldPos(i);

            GameObject instantiated = Instantiate(targetPrefab, worldPos, Quaternion.identity, transform);
            spawnedTargets.Add(instantiated);
            totalTargetsSpawned++;
        }

        StartCoroutine(SpawnTargetsOverTime());
    }

    IEnumerator SpawnTargetsOverTime()
    {
        while (totalTargetsSpawned < targetCount)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);   
            int segmentIdx = Random.Range(0, screenSegments);
            Vector2 worldPos = GetRandomSpawnWorldPos(segmentIdx);
            
            GameObject instantiated = Instantiate(targetPrefab, worldPos, Quaternion.identity, transform);
            spawnedTargets.Add(instantiated);
            totalTargetsSpawned++;
        }
    }

    void DestroyTargets()
    {
        StopCoroutine(SpawnTargetsOverTime());
        foreach (var target in spawnedTargets)
        {
            Destroy(target);
        }
        totalTargetsSpawned = 0;
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

    void TryRemoveNullTargets()
    {
        spawnedTargets.RemoveAll(t => t == null);
    }
}
