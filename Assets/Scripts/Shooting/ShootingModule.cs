using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShootingModule : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private Image crosshair;
    // [SerializeField] private Transform gunVisualsTransform;
    private PlayerControls controls;
    private PlayerRuntimeStats playerRuntimeStats;

    [Header("Guns")]
    private Gun currGun;

    [Header("Config")]
    [SerializeField] private float bullseyeDistanceThreshold;
    [SerializeField] private float ricochetDistance;
    [SerializeField] private Vector3 aerialStrikeOriginOffset = new(-2f, 20f, 0f);
    [SerializeField] private bool enableAerialStrikeShake = true;
    [SerializeField] private bool enableBulletShake = true;

    [Header("Chance Bags")]
    [SerializeField] private ChanceBag aerialStrikeChanceBag;
    [SerializeField] private ChanceBag bullseyeChanceBag;

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

    [Header("Actions")]
    public Action OnGunInitialized;

    void Awake()
    {
        controls = new();
    }

    void Start()
    {
        playerRuntimeStats = GameManager.Instance.PlayerRuntimeStats;
        EquipGun(GameManager.Instance.CurrGunType);
        crosshair.color = playerRuntimeStats.ActiveCrosshairColor;

        EnableShooting();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        if (!autoFire)
            controls.Player.Shoot.performed += ManualShoot;
        RoundManager.OnRoundEnd += DisableShooting;
    }

    void OnDisable()
    {
        controls.Player.Disable();
        if (!autoFire)
            controls.Player.Shoot.performed -= ManualShoot;

        RoundManager.OnRoundEnd -= DisableShooting;
    }

    private void EquipGun(GunType gunType)
    {
        if (currGun != null)
            Destroy(currGun.gameObject);

        switch (gunType)
        {
            case GunType.Pistol:
                break;
            case GunType.Shotgun:
                break;
            case GunType.Sniper:
                break;
        }

        GameObject gunGO = Instantiate(GameManager.Instance.GunDictionary[gunType], transform);
        currGun = gunGO.GetComponent<Gun>();

        if (currGun == null)
        {
            Debug.LogError($"ShootingModule: prefab '{gunGO.name}' has no Gun component attached.");
            return;
        }

        currGun.InitializeGunData();
        GameManager.Instance.CurrGunInstance = currGun;
        OnGunInitialized?.Invoke();
    }

    void LateUpdate()
    {
        UpdateCrosshair();
    }

    void EnableShooting()
    {
        crosshair.gameObject.SetActive(true);
        Cursor.visible = false;

        if (autoFire)
            currFireRateCoroutine = StartCoroutine(HandleFireRate());
        else
            controls.Player.Shoot.performed += ManualShoot;
    }

    public void DisableShooting()
    {
        crosshair.gameObject.SetActive(false);
        Cursor.visible = true;

        if (autoFire)
            StopCoroutine(currFireRateCoroutine);
        else
            controls.Player.Shoot.performed -= ManualShoot;
    }

    void UpdateCrosshair()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        crosshair.rectTransform.position = screenPos;
    }

    private IEnumerator HandleFireRate()
    {
        // if reloading --> wait for reload to return before we try shoot again
        if (reloadCoroutine != null)
        {
            yield return reloadCoroutine;
        }
        else if (aerialStrikeCoroutine != null)
        {
            yield return aerialStrikeCoroutine;
            yield return AutoFireCrosshairAnim();
        }
        else
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
            1f / currGun.AutoFireRate
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
        if (currGun.IsReloading) return; // Can't shoot while reloading

        if (currGun.CurrAmmo == 0)
        {
            reloadCoroutine = StartCoroutine(Reload());
            return;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        var hit = Physics2D.GetRayIntersection(ray);

        List<Target> hits = new();
        List<bool> isBullseyes = new();

        if (hit.collider != null)
        {
            bool isBullseye;
            if (randomBullseye)
            {
                isBullseye = bullseyeChanceBag.Pull(currGun.BullseyeChance);
            }
            else
            {
                // Calculate the distance to the bullseye
                float distToBullseye = Vector2.Distance(worldPos, hit.collider.transform.position);
                isBullseye = distToBullseye < bullseyeDistanceThreshold;
            }

            Target target = hit.collider.GetComponent<Target>();

            hits.Add(target);
            isBullseyes.Add(isBullseye);

            currGun.HandleRicochetShot(target, ref hits, ref isBullseyes, ricochetDistance);
        }
        else
        {
            hits.Add(null);
            isBullseyes.Add(false);
        }

        StartCoroutine(currGun.HandleShot(worldPos, hits, isBullseyes));
        aerialStrikeCoroutine = StartCoroutine(HandleAerialStrike(hits));

        // CurrAmmo--;
        if (currGun.CurrAmmo == 0)
        {
            reloadCoroutine = StartCoroutine(Reload());
            return;
        }
    }

    private IEnumerator Reload()
    {
        StartCoroutine(currGun.Reload());
        yield return BasicAnimations.Interpolate(
            () =>
            {
                crosshair.color = playerRuntimeStats.InactiveCrosshairColor;
                autoFireCrosshairCountdown.fillAmount = 1f;
            },
            (t) =>
            {
                autoFireCrosshairCountdown.fillAmount = 1 - t;
            },
            () =>
            {
                crosshair.color = playerRuntimeStats.ActiveCrosshairColor;
                autoFireCrosshairCountdown.fillAmount = 0f;
            },
            currGun.ReloadTime
        );
        reloadCoroutine = null;
    }

    private IEnumerator HandleAerialStrike(List<Target> shotTargets)
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
            List<Target> currTargetsOnScreen = new(targetSpawner.SpawnedTargets);
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

                yield return currGun.Shoot(tracerStartPos, targetPos, true, target, false, false);

                if (enableAerialStrikeShake)
                    Camera.main.GetComponent<CameraShake>().Explosion(0.5f, 0.1f);
            }
        }

        aerialStrikeCoroutine = null;
    }
}
