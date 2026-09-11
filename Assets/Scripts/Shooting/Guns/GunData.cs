using System;
using UnityEngine;

[Serializable]
public class GunData
{
    public float Damage;
    public int MaxAmmo;
    public float ReloadTime;
    public float AutoFireRate; // measured in bullets per second
    [Range(0, 1)] public float CritChance;
    [Range(0, 1)] public float BullseyeChance;

    public GunData(GunDataSO so)
    {
        Damage = so.Damage;
        MaxAmmo = so.MaxAmmo;
        ReloadTime = so.ReloadTime;
        AutoFireRate = so.AutoFireRate;
        CritChance = so.CritChance;
        BullseyeChance = so.BullseyeChance;
    }
}
