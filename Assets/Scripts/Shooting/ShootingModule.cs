using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShootingModule : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private Image crosshair;
    [SerializeField] private Image reloadImage;
    private PlayerControls controls;
    private PlayerRuntimeStats playerRuntimeStats;

    [Header("Bullet Trail")]
    [SerializeField] private TrailRenderer bulletTrailPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float trailSpeed;


    [Header("Config")]
    [SerializeField] private float bullseyeDistanceThreshold;
    [SerializeField] private float ricochetDistance;

    [Space(20)]
    private int currAmmo;
    [HideInInspector] public int CurrAmmo
    {
        get => currAmmo;
        set
        {
            if (currAmmo == value) return; // if value didn't change don't do anything
            currAmmo = value;
            OnAmmoValueChanged.Invoke(currAmmo);
        }
    }
    private bool isReloading;

    [Header("Chance Bags")]
    [SerializeField] private ChanceBag ricochetChanceBag;

    [Space(20)]

    [Header("Actions")]
    public Action<GameObject, bool> ShotFired;
    public Action<int> OnAmmoValueChanged;

    void Awake()
    {
        controls = new();
    }

    void Start()
    {
        playerRuntimeStats = GameManager.Instance.PlayerRuntimeStats;
        EnableShooting();
        CurrAmmo = playerRuntimeStats.MaxAmmo;
        crosshair.color = playerRuntimeStats.ActiveCrosshairColor;

        reloadImage.color = Color.white;
        reloadImage.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Shoot.performed += TryShoot;

        targetSpawner.OnTargetsCleared += DisableShooting;
    }

    void OnDisable()
    {   
        controls.Player.Disable();
        controls.Player.Shoot.performed -= TryShoot;
    
        targetSpawner.OnTargetsCleared -= DisableShooting;
    }

    void Update()
    {
        UpdateCrosshair();
    }

    void EnableShooting()
    {
        crosshair.gameObject.SetActive(true);
        Cursor.visible = false;
        controls.Player.Shoot.performed += TryShoot;
    }

    public void DisableShooting()
    {
        crosshair.gameObject.SetActive(false);
        Cursor.visible = true;
        controls.Player.Shoot.performed -= TryShoot;
    }

    // void TryShoot(InputAction.CallbackContext context)
    // {
    //     if (isReloading) return; // Can't shoot while reloading

    //     if (CurrAmmo == 0)
    //     {
    //         StartCoroutine(TryReload());
    //         return;
    //     }

    //     Vector2 screenPos = Mouse.current.position.ReadValue();
    //     Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
    //     Ray ray = Camera.main.ScreenPointToRay(screenPos);
    //     var hit = Physics2D.GetRayIntersection(ray);

    //     GameObject target = null;
    //     bool isBullseye = false;
        
    //     if (hit.collider != null)
    //     {
    //         // Calculate the distance to the bullseye
    //         float distToBullseye = Vector2.Distance(worldPos, hit.collider.transform.position);
    //         isBullseye = distToBullseye < bullseyeDistanceThreshold;
    //         target = hit.collider.gameObject;


    //         StartCoroutine(HandleRicochetShot(target, worldPos));

    //         // Destroy(target);
    //     }
  
    //     // StartCoroutine(SpawnTracer(Camera.main.ScreenToWorldPoint(muzzlePoint.position), worldPos));
    //     // StartCoroutine(RequestDestroyTarget(target, isBullseye));
    //     StartCoroutine(DestroyTargetAfterTracer(Camera.main.ScreenToWorldPoint(muzzlePoint.position), worldPos, false, target, isBullseye));

    //     CurrAmmo--;
    //     // ShotFired?.Invoke(target, isBullseye);
        

    //     if (CurrAmmo == 0)
    //     {
    //         StartCoroutine(TryReload());
    //         return;   
    //     }
    // }

    void TryShoot(InputAction.CallbackContext context)
    {
        if (isReloading) return; // Can't shoot while reloading

        if (CurrAmmo == 0)
        {
            StartCoroutine(TryReload());
            return;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        var hit = Physics2D.GetRayIntersection(ray);

        GameObject target = null;
        bool isBullseye = false;

        List<GameObject> hits = new();
        List<bool> isBullseyes = new();
        
        if (hit.collider != null)
        {
            // Calculate the distance to the bullseye
            float distToBullseye = Vector2.Distance(worldPos, hit.collider.transform.position);
            isBullseye = distToBullseye < bullseyeDistanceThreshold;
            target = hit.collider.gameObject;

            hits.Add(target);
            isBullseyes.Add(isBullseye);

            HandleRicochetShot(target, ref hits, ref isBullseyes);
        } else
        {
            hits.Add(null);
            isBullseyes.Add(false);
        }

        // hits.Prepend(target);
        // isBullseyes.Prepend(isBullseye);

        // Debug.Log(hits.Count);
        // Debug.Log(isBullseyes.Count);

        // Debug.Log(hits.ToSeparatedString(", "));
        // Debug.Log(isBullseyes.ToSeparatedString(", "));
  
        StartCoroutine(HandleShotFired(worldPos, hits, isBullseyes));

        CurrAmmo--;
        if (CurrAmmo == 0)
        {
            StartCoroutine(TryReload());
            return;   
        }
    }

    private IEnumerator HandleShotFired(Vector2 shotPos, List<GameObject> hits, List<bool> isBullseyes)
    {
        Vector3 muzzlePointInWorldSpace = Camera.main.ScreenToWorldPoint(muzzlePoint.position);
        Vector3 startPos = new(muzzlePointInWorldSpace.x, muzzlePointInWorldSpace.y, 0);
        Vector3 shotPosVec3 = new(shotPos.x, shotPos.y, 0);
        yield return SpawnTracer(startPos, shotPosVec3);
        if (hits[0] != null)
            Destroy(hits[0]);
        ShotFired?.Invoke(hits[0], isBullseyes[0]);

        for (int i = 1; i < hits.Count; i++)
        {
            if (hits[i-1] == null) continue;
            Vector3 tracerStartPos = hits[i-1].transform.position;
            tracerStartPos.z = 0;
            if (i == 1) tracerStartPos = shotPosVec3;
            yield return SpawnTracer(tracerStartPos, hits[i].transform.position);
            if (hits[i] != null)
                Destroy(hits[i]);
            ShotFired?.Invoke(hits[i], isBullseyes[i]);
        }

        yield return null;
    }

    // private IEnumerator HandleRicochetShot(GameObject target, Vector2 shotPos)
    // {
    //     if (playerRuntimeStats.RicochetShotChance == 0f)
    //     {
    //         ricochetChanceBag.Clear();
    //         // return;
    //         yield return null;
    //     }

    //     targetSpawner.IsPositionClear(target.transform.position, ricochetDistance, out Collider2D[] hits);
        
    //     GameObject firstHit = null;
    //     if (hits.Length >= 2)
    //     {
    //         foreach (var hit in hits)
    //         {
    //             if (hit.gameObject == target) continue;
    //             firstHit = hit.gameObject;
    //             break;
    //         }
    //     }

    //     if (firstHit != null)
    //     {
    //         if (ricochetChanceBag.IsEmpty)
    //             ricochetChanceBag.NewBag(playerRuntimeStats.RicochetShotChance);

    //         bool ricochet = ricochetChanceBag.Pull();
    //         if (ricochet)
    //         {
    //             // yield return SpawnTracer(shotPos, firstHit.transform.position, true);
    //             // Destroy(firstHit);
    //             // ShotFired?.Invoke(firstHit, false);

    //             StartCoroutine(DestroyTargetAfterTracer(shotPos, firstHit.transform.position, true, firstHit, false));

    //             // StartCoroutine(RequestDestroyTarget(firstHit, false));
    //             // return;
    //             yield return null;
    //         }
    //     }
    // }

    private void HandleRicochetShot(GameObject target, ref List<GameObject> hits, ref List<bool> isBullseyes)
    {
        if (playerRuntimeStats.RicochetShotChance == 0f)
        {
            ricochetChanceBag.Clear();
            return;
        }

        targetSpawner.IsPositionClear(target.transform.position, ricochetDistance, out Collider2D[] colliders);
        
        GameObject firstHit = null;
        if (colliders.Length >= 2)
        {
            foreach (var collider in colliders)
            {
                // if (collider.gameObject == target) continue;
                if (hits.Contains(collider.gameObject)) continue;
                firstHit = collider.gameObject;
                break;
            }
        }

        if (firstHit != null)
        {
            if (ricochetChanceBag.IsEmpty)
                ricochetChanceBag.NewBag(playerRuntimeStats.RicochetShotChance);

            bool ricochet = ricochetChanceBag.Pull();
            if (ricochet)
            {
                hits.Add(firstHit);
                isBullseyes.Add(false); // TODO: CHANGE WHEN I WANT RICOCHETS TO HIT BULLSEYES TOO

                HandleRicochetShot(firstHit, ref hits, ref isBullseyes);
            }
        }
    }

    IEnumerator TryReload()
    {
        yield return BasicAnimations.Interpolate(
            () =>
            {
                isReloading = true;
                crosshair.color = playerRuntimeStats.InactiveCrosshairColor;
                
                reloadImage.gameObject.SetActive(true);
            },
            (t) =>
            {
                reloadImage.fillAmount = t;
            },
            () =>
            {
                CurrAmmo = playerRuntimeStats.MaxAmmo;
                isReloading = false;
                crosshair.color = playerRuntimeStats.ActiveCrosshairColor;

                reloadImage.gameObject.SetActive(false);
            },
            playerRuntimeStats.ReloadTime
        );
    }

    void UpdateCrosshair()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        crosshair.rectTransform.position = screenPos;
    }

    // private IEnumerator DestroyTargetAfterTracer(Vector2 startPos, Vector2 targetPos, bool isRicochet, GameObject hit, bool isBullseye)
    // {
    //     yield return SpawnTracer(startPos, targetPos, isRicochet);
    //     // StartCoroutine(RequestDestroyTarget(hit, isBullseye));
    //     if (hit != null)
    //         Destroy(hit);
    //     ShotFired?.Invoke(hit, isBullseye);

    // }

    private IEnumerator SpawnTracer(Vector3 startPos, Vector3 targetPosition)
    {
        // if (isRicochet)
        // {
        //     yield return new WaitForSeconds(bulletTrailPrefab.time);
        // }

        // Instantiate the tracer at the muzzle position
        // Vector2 spawnPosition = startPos;
        TrailRenderer tracer = Instantiate(bulletTrailPrefab, startPos, Quaternion.identity);
        
        Vector3 startPosition = tracer.transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float remainingDistance = distance;

        // Move the tracer towards the destination smoothly based on distance/speed
        while (remainingDistance > 0)
        {
            tracer.transform.position = Vector3.MoveTowards(
                tracer.transform.position, 
                targetPosition, 
                trailSpeed * Time.deltaTime
            );

            remainingDistance = Vector3.Distance(tracer.transform.position, targetPosition);
            yield return null; 
        }

        tracer.transform.position = targetPosition;
    }

    // private IEnumerator RequestDestroyTarget(GameObject hit, bool isBullseye)
    // {
    //     // yield return new WaitForSeconds(bulletTrailPrefab.time);
    //     if (hit != null)
    //         Destroy(hit);
    //     ShotFired?.Invoke(hit, isBullseye);
    // }
}
