using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform gunVisualsTransform;
    [SerializeField] private GameObject targetHitParticles;
    [SerializeField] private Flytext flytextPrefab;
    private PlayerControls controls;
    private PlayerRuntimeStats playerRuntimeStats;

    [Header("Bullet Trail")]
    [SerializeField] private TrailRenderer bulletTrailPrefab;
    [SerializeField] private TrailRenderer aerialStrikeTrailPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float bulletTrailSpeed;
    [SerializeField] private float aerialStrikeTrailSpeed = 50f;


    [Header("Config")]
    [SerializeField] private float bullseyeDistanceThreshold;
    [SerializeField] private float ricochetDistance;
    [SerializeField] private Vector3 aerialStrikeOriginOffset = new(-2f, 20f, 0f);
    [SerializeField] private bool enableAerialStrikeShake = true;
    [SerializeField] private bool enableBulletShake = true;
    [SerializeField] private Color bullseyeFlytextColor;

    [Space(20)]
    private int currAmmo;
    [HideInInspector]
    public int CurrAmmo
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
    [SerializeField] private ChanceBag ricochetBullseyeChanceBag;
    [SerializeField] private ChanceBag aerialStrikeChanceBag;
    [SerializeField] private ChanceBag bullseyeChanceBag;

    [Space(20)]

    [Header("Actions")]
    public Action<GameObject, bool> ShotFired; // GameObject: target, 1st bool: isBullseye
    public Action<int> OnAmmoValueChanged;

    [Header("Auto Fire Settings")]
    [SerializeField] private Image autoFireCrosshairCountdown;
    private Coroutine currFireRateCoroutine;
    private Coroutine reloadCoroutine;

    [Header("Aerial Strike Settings")]
    [SerializeField] private int aerialStrikeTargetThreshold = 5;
    private bool isAerialStrikeDue;
    private Coroutine aerialStrikeCoroutine;

    [Header("Debug Settings")]
    [SerializeField] private bool autoFire;
    [SerializeField] private bool randomBullseye;

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
        if (!autoFire)
            controls.Player.Shoot.performed += ManualShoot;

        targetSpawner.OnTargetsCleared += DisableShooting;
    }

    void OnDisable()
    {
        controls.Player.Disable();
        if (!autoFire)
            controls.Player.Shoot.performed -= ManualShoot;

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
        if (autoFire)
        {
            currFireRateCoroutine = StartCoroutine(HandleFireRate());
        }
        else
        {
            controls.Player.Shoot.performed += ManualShoot;
        }
    }


    public void DisableShooting()
    {
        crosshair.gameObject.SetActive(false);
        Cursor.visible = true;
        if (autoFire)
        {
            StopCoroutine(currFireRateCoroutine);
        }
        else
        {
            controls.Player.Shoot.performed -= ManualShoot;
        }
    }

    private IEnumerator HandleFireRate()
    {
        // if reloading --> wait for reload to return before we try shoot again
        if (reloadCoroutine != null)
        {
            yield return reloadCoroutine;
        } else if (aerialStrikeCoroutine != null)
        {
            yield return aerialStrikeCoroutine;
            yield return AutoFireCrosshairAnim();
        } else
        {
            yield return AutoFireCrosshairAnim();
        }

        TryShoot();
        currFireRateCoroutine = StartCoroutine(HandleFireRate());
    }

    private IEnumerator AutoFireCrosshairAnim()
    {
        yield return BasicAnimations.Interpolate(
            () =>
            {
                autoFireCrosshairCountdown.fillAmount = 1;
            },
            (t) =>
            {
                autoFireCrosshairCountdown.fillAmount = 1 - t;
            },
            () =>
            {
                autoFireCrosshairCountdown.fillAmount = 0;
            },
            1f / playerRuntimeStats.AutoFireRate
        );
    }

    /// <summary>
    /// This is a helper method to call TryShoot when autoFire is turned off. This is needed because subscribing to an input action
    /// requires an InputAction.CallbackContext
    /// </summary>
    /// <param name="context"></param>
    private void ManualShoot(InputAction.CallbackContext context)
    {
        TryShoot();
    }

    void TryShoot()
    {
        if (isReloading) return; // Can't shoot while reloading

        if (CurrAmmo == 0)
        {
            reloadCoroutine = StartCoroutine(TryReload());
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
            if (randomBullseye)
            {
                isBullseye = playerRuntimeStats.BullseyeChance == 0f
                    ? false
                    : bullseyeChanceBag.Pull(playerRuntimeStats.BullseyeChance);
            }
            else
            {
                // Calculate the distance to the bullseye
                float distToBullseye = Vector2.Distance(worldPos, hit.collider.transform.position);
                isBullseye = distToBullseye < bullseyeDistanceThreshold;
            }

            target = hit.collider.gameObject;

            hits.Add(target);
            isBullseyes.Add(isBullseye);

            HandleRicochetShot(target, ref hits, ref isBullseyes);
        }
        else
        {
            hits.Add(null);
            isBullseyes.Add(false);
        }

        StartCoroutine(HandleShotFired(worldPos, hits, isBullseyes));
        aerialStrikeCoroutine = StartCoroutine(HandleAerialStrike(hits));

        CurrAmmo--;
        if (CurrAmmo == 0)
        {
            reloadCoroutine = StartCoroutine(TryReload());
            return;
        }
    }

    private IEnumerator HandleShotFired(Vector2 shotPos, List<GameObject> hits, List<bool> isBullseyes)
    {
        // Vector3 muzzlePointInWorldSpace = Camera.main.ScreenToWorldPoint(muzzlePoint.position);
        Vector3 muzzlePointInWorldSpace = muzzlePoint.position;
        Vector3 startPos = new(muzzlePointInWorldSpace.x, muzzlePointInWorldSpace.y, 0);
        Vector3 shotPosVec3 = new(shotPos.x, shotPos.y, 0);
        SFXManager.PlaySound(SoundType.Gunshot);
        if (enableBulletShake)
            Camera.main.GetComponent<CameraShake>().Recoil(new(0f, -1f), 0.15f, gunVisualsTransform);
        // yield return SpawnTracer(startPos, shotPosVec3);
        // if (hits[0] != null)
        // {
        //     Instantiate(targetHitParticles, shotPosVec3, Quaternion.identity);
        //     SFXManager.PlaySound(SoundType.TargetHit);
        //     Destroy(hits[0]);
        // }
        // ShotFired?.Invoke(hits[0], isBullseyes[0]);
        yield return DestroyTarget(startPos, shotPosVec3, hits[0], isBullseyes[0]);

        for (int i = 1; i < hits.Count; i++)
        {
            if (hits[i - 1] == null) continue;
            Vector3 tracerStartPos = hits[i - 1].transform.position;
            tracerStartPos.z = 0;
            if (i == 1) tracerStartPos = shotPosVec3;
            yield return DestroyTarget(tracerStartPos, hits[i].transform.position, hits[i], isBullseyes[i]);
            // yield return SpawnTracer(tracerStartPos, hits[i].transform.position);
            // if (hits[i] != null)
            // {
            //     Instantiate(targetHitParticles, hits[i].transform.position, Quaternion.identity);
            //     SFXManager.PlaySound(SoundType.TargetHit);

            //     Destroy(hits[i]);
            // }
            // ShotFired?.Invoke(hits[i], isBullseyes[i]);
        }
    }

    private IEnumerator DestroyTarget(Vector3 tracerStartPos, Vector3 tracerEndPos, GameObject hit, bool isBullseye)
    {
        yield return SpawnTracer(tracerStartPos, tracerEndPos);
        if (hit != null)
        {
            Instantiate(targetHitParticles, tracerEndPos, Quaternion.identity);
            SFXManager.PlaySound(SoundType.TargetHit);
            Destroy(hit);

            if (isBullseye)
            {
                Flytext flytext = Instantiate(flytextPrefab, hit.transform.position, Quaternion.identity);
                flytext.Show("bullseye!", 1f, Vector2.up, bullseyeFlytextColor);
            }
        }
        ShotFired?.Invoke(hit, isBullseye);
    }

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
                if (hits.Contains(collider.gameObject)) continue;
                firstHit = collider.gameObject;
                break;
            }
        }

        if (firstHit != null)
        {
            bool ricochet = ricochetChanceBag.Pull(playerRuntimeStats.RicochetShotChance);
            if (ricochet)
            {
                hits.Add(firstHit);
                bool isBullseye = false;
                if (playerRuntimeStats.RicochetBullseyeChance != 0f)
                {
                    isBullseye = ricochetBullseyeChanceBag.Pull(playerRuntimeStats.RicochetBullseyeChance);
                }
                isBullseyes.Add(isBullseye);

                // hits.Count-1 gives you the curr number of ricochet bounces. only if this is less than RicochetMaxBounce then handle another ricochet shot
                if ((hits.Count - 1) < playerRuntimeStats.RicochetMaxBounce)
                    HandleRicochetShot(firstHit, ref hits, ref isBullseyes);
            }
        }
    }

    private IEnumerator HandleAerialStrike(List<GameObject> shotTargets)
    {
        if (playerRuntimeStats.AerialStrikeChance == 0f)
        {
            aerialStrikeChanceBag.Clear();
            aerialStrikeCoroutine = null;
            yield break;
        }

        bool aerialStrike = false;
        if (!isAerialStrikeDue) // only pull if we don't already have an aerial strike in queue 
            aerialStrike = aerialStrikeChanceBag.Pull(playerRuntimeStats.AerialStrikeChance);
        
        if (aerialStrike || isAerialStrikeDue)
        {
            List<GameObject> currTargetsOnScreen = new(targetSpawner.SpawnedTargets);
            isAerialStrikeDue = currTargetsOnScreen.Count <= aerialStrikeTargetThreshold;
            if (isAerialStrikeDue)  // If there are too few targets then don't perform aerial strike but queue it up 
            {
                aerialStrikeCoroutine = null;
                yield break;
            }
            SFXManager.PlaySound(SoundType.AerialStrike);

            foreach (var target in currTargetsOnScreen)
            {
                if (target == null) continue;
                if (shotTargets.Contains(target)) continue;
                Vector3 targetPos = target.transform.position;
                targetPos.z = 0; // make sure targetPos.z is 0
                Vector3 tracerStartPos = targetPos + aerialStrikeOriginOffset;
                yield return SpawnTracer(tracerStartPos, targetPos, true);
                SFXManager.PlaySound(SoundType.TargetHit);
                Destroy(target);
                ShotFired?.Invoke(target, false);
                if (enableAerialStrikeShake)
                    Camera.main.GetComponent<CameraShake>().Explosion(0.5f, 0.1f);
            }
        }

        aerialStrikeCoroutine = null;
    }

    IEnumerator TryReload()
    {
        bool sfxPlayed = false;
        yield return BasicAnimations.Interpolate(
            () =>
            {
                isReloading = true;
                crosshair.color = playerRuntimeStats.InactiveCrosshairColor;

                // reloadImage.gameObject.SetActive(true);
                autoFireCrosshairCountdown.fillAmount = 1f;
            },
            (t) =>
            {
                // TODO: REPLACE THIS TEMPORARY SFX PLAY
                if (t >= 0.2f && !sfxPlayed)
                {
                    sfxPlayed = true;
                    SFXManager.PlaySound(SoundType.ReloadGun);
                }

                autoFireCrosshairCountdown.fillAmount = 1 - t;
            },
            () =>
            {
                CurrAmmo = playerRuntimeStats.MaxAmmo;
                isReloading = false;
                crosshair.color = playerRuntimeStats.ActiveCrosshairColor;

                // reloadImage.gameObject.SetActive(false);
                autoFireCrosshairCountdown.fillAmount = 0f;
            },
            playerRuntimeStats.ReloadTime
        );
        reloadCoroutine = null;
    }

    void UpdateCrosshair()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        crosshair.rectTransform.position = screenPos;
    }

    private IEnumerator SpawnTracer(Vector3 startPos, Vector3 targetPosition, bool isAerialStrike = false)
    {
        TrailRenderer prefab = isAerialStrike ? aerialStrikeTrailPrefab : bulletTrailPrefab;
        TrailRenderer tracer = Instantiate(prefab, startPos, Quaternion.identity);

        Vector3 startPosition = tracer.transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float remainingDistance = distance;
        float speed = isAerialStrike ? aerialStrikeTrailSpeed : bulletTrailSpeed;

        // Move the tracer towards the destination smoothly based on distance/speed
        while (remainingDistance > 0)
        {
            tracer.transform.position = Vector3.MoveTowards(
                tracer.transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            remainingDistance = Vector3.Distance(tracer.transform.position, targetPosition);
            yield return null;
        }

        tracer.transform.position = targetPosition;
    }
}
