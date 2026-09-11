using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

[Serializable]
[AutoStaticsCleanup]
public abstract partial class Gun : MonoBehaviour
{
    [Header("Gun Data")]
    [SerializeField] protected GunDataSO baseGunData;
    [HideInInspector] public float Damage;
    [HideInInspector] public int MaxAmmo;
    [HideInInspector] public float ReloadTime;
    [HideInInspector] public float AutoFireRate; // measured in bullets per second
    [HideInInspector] public int RicochetMaxBounce;
    [HideInInspector] [Range(0, 1)] public float CritChance;
    [HideInInspector] [Range(0, 1)] public float BullseyeChance;
    [Space(10)]
    [HideInInspector] public bool IsReloading;
    [HideInInspector] public int CurrAmmo;

    [Header("References")]
    [SerializeField] protected Transform muzzlePoint;
    [SerializeField] protected SoundType gunshotSoundType;

    [Header("Chance Bags")]
    [SerializeField] protected ChanceBag ricochetChanceBag;
    [SerializeField] protected ChanceBag ricochetBullseyeChanceBag;
    [SerializeField] protected ChanceBag critChanceBag;

    [Header("Actions")]
    public static Action<Target, bool, bool, bool, Vector3> ShotFired;
    public static Action<int> OnAmmoValueChanged;

    [Header("Bullet Trail")]
    [SerializeField] protected TrailRenderer bulletTrailPrefab;
    [SerializeField] protected float bulletTrailSpeed = 200f;
    [SerializeField] protected TrailRenderer aerialStrikeTrailPrefab;
    [SerializeField] protected float aerialStrikeTrailSpeed = 300f;

    protected virtual void Awake()
    {
        InitializeGunData();
        CurrAmmo = MaxAmmo;
    }

    public virtual void InitializeGunData()
    {
        Damage = baseGunData.Damage;
        MaxAmmo = baseGunData.MaxAmmo;
        ReloadTime = baseGunData.ReloadTime;
        AutoFireRate = baseGunData.AutoFireRate;
        RicochetMaxBounce = baseGunData.RicochetMaxBounce;
        CritChance = baseGunData.CritChance;
        BullseyeChance = baseGunData.BullseyeChance;

        // TODO: HANDLE UPGRADES
        CheckForUpgrades();
    }

    protected virtual void CheckForUpgrades()
    {
        Damage += GameManager.Instance.UpgradesData.Damage;
        MaxAmmo += GameManager.Instance.UpgradesData.MaxAmmo;
        ReloadTime += GameManager.Instance.UpgradesData.ReloadTime;
        AutoFireRate += GameManager.Instance.UpgradesData.AutoFireRate;
        RicochetMaxBounce += GameManager.Instance.UpgradesData.RicochetMaxBounce;
        CritChance += GameManager.Instance.UpgradesData.CritChance;
        BullseyeChance += GameManager.Instance.UpgradesData.BullseyeChance;
        
        GameManager.Instance.PlayerRuntimeStats.AerialStrikeChance += MaxAmmo * GameManager.Instance.UpgradesData.AerialStrikeChanceMaxAmmo;
    }

    protected virtual void OnDisable()
    {
        Damage -= GameManager.Instance.UpgradesData.Damage;
        MaxAmmo -= GameManager.Instance.UpgradesData.MaxAmmo;
        ReloadTime -= GameManager.Instance.UpgradesData.ReloadTime;
        AutoFireRate -= GameManager.Instance.UpgradesData.AutoFireRate;
        RicochetMaxBounce -= GameManager.Instance.UpgradesData.RicochetMaxBounce;
        CritChance -= GameManager.Instance.UpgradesData.CritChance;
        BullseyeChance -= GameManager.Instance.UpgradesData.BullseyeChance;
        
        GameManager.Instance.PlayerRuntimeStats.AerialStrikeChance -= MaxAmmo * GameManager.Instance.UpgradesData.AerialStrikeChanceMaxAmmo;
    }

    public virtual IEnumerator HandleShot(Vector2 shotPos, List<Target> hits, List<bool> isBullseyes)
    {
        CurrAmmo--;
        OnAmmoValueChanged?.Invoke(CurrAmmo);

        Vector3 muzzlePointInWorldSpace = muzzlePoint.position;
        Vector3 startPos = new(muzzlePointInWorldSpace.x, muzzlePointInWorldSpace.y, 0);
        Vector3 shotPosVec3 = new(shotPos.x, shotPos.y, 0);

        SFXManager.PlaySound(gunshotSoundType);

        // TODO: ADD TOGGLE TO ENABLE/DISABLE BULLET SHAKE WHEN I IMPLEMENT PLAYER SETTINGS
        // if (enableBulletShake)
        Camera.main.GetComponent<CameraShake>().Recoil(new(0f, -1f), 0.15f, transform);
        
        yield return Shoot(startPos, shotPosVec3, false, hits[0], isBullseyes[0]);

        for (int i = 1; i < hits.Count; i++)
        {
            if (hits[i - 1] == null) continue;
            Vector3 tracerStartPos = hits[i - 1].transform.position;
            tracerStartPos.z = 0;
            if (i == 1) tracerStartPos = shotPosVec3;
            yield return Shoot(tracerStartPos, hits[i].transform.position, false, hits[i], isBullseyes[i]);
        } 
    }

    public virtual IEnumerator Shoot(Vector3 tracerStartPos, Vector3 tracerEndPos, bool isAerialStrike, Target hit, bool isBullseye)
    {
        yield return SpawnTracer(tracerStartPos, tracerEndPos, isAerialStrike);
        bool isCrit = critChanceBag.Pull(CritChance) && !isAerialStrike;

        ShotFired?.Invoke(hit, isBullseye, isCrit, isAerialStrike, tracerEndPos);
    }

    public virtual void HandleRicochetShot(Target target, ref List<Target> hits, ref List<bool> isBullseyes, float ricochetDistance)
    {
        if (GameManager.Instance.PlayerRuntimeStats.RicochetShotChance == 0f)
        {
            ricochetChanceBag.Clear();
            return;
        }

        Collider2D[] colliders = target.FindNearbyTargets(ricochetDistance);

        Target firstHit = null;
        if (colliders.Length >= 2)
        {
            Debug.Log("1");

            foreach (var collider in colliders)
            {
                if (hits.Contains(collider.GetComponent<Target>())) continue;
                firstHit = collider.GetComponent<Target>();
                break;
            }
        }

        if (firstHit != null)
        {
            bool ricochet = ricochetChanceBag.Pull(GameManager.Instance.PlayerRuntimeStats.RicochetShotChance);
            Debug.Log("2");
            if (ricochet)
            {
                hits.Add(firstHit);
                bool isBullseye = ricochetBullseyeChanceBag.Pull(GameManager.Instance.PlayerRuntimeStats.RicochetBullseyeChance);
                isBullseyes.Add(isBullseye);

                // hits.Count-1 gives you the curr number of ricochet bounces. only if this is less than RicochetMaxBounce then handle another ricochet shot
                if ((hits.Count - 1) < RicochetMaxBounce)
                    HandleRicochetShot(firstHit, ref hits, ref isBullseyes, ricochetDistance);
            }
        }
    }

    public virtual IEnumerator Reload()
    {
        bool sfxPlayed = false;
        yield return BasicAnimations.Interpolate(
            () =>
            {
                IsReloading = true;
            },
            (t) =>
            {
                // TODO: REPLACE THIS TEMPORARY SFX PLAY
                if (t >= 0.2f && !sfxPlayed)
                {
                    sfxPlayed = true;
                    SFXManager.PlaySound(SoundType.ReloadGun);
                }
            },
            () =>
            {
                CurrAmmo = MaxAmmo;
                IsReloading = false;
            },
            ReloadTime
        );    }

    public virtual IEnumerator SpawnTracer(Vector3 startPos, Vector3 targetPosition, bool isAerialStrike = false)
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
