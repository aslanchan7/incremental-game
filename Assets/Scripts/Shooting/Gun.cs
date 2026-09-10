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
    [SerializeField] protected float bulletTrailSpeed;
    [SerializeField] protected TrailRenderer aerialStrikeTrailPrefab;
    [SerializeField] protected float aerialStrikeTrailSpeed = 50f;

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
        // CheckForUpgrades();
    }

    public abstract IEnumerator HandleShot(Vector2 shotPos, List<Target> hits, List<bool> isBullseyes);

    public abstract IEnumerator Shoot(Vector3 tracerStartPos, Vector3 tracerEndPos, bool isAerialStrike, Target hit, bool isBullseye);

    public abstract void HandleRicochetShot(Target target, ref List<Target> hits, ref List<bool> isBullseyes, float ricochetDistance);

    public abstract IEnumerator Reload();

    public abstract IEnumerator SpawnTracer(Vector3 startPos, Vector3 targetPosition, bool isAerialStrike = false);
}
