using System;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "UpgradesData", menuName = "Data/Upgrades Data")]
public class UpgradesData : ScriptableObject
{
    public float Damage;
    public int MaxAmmo;
    public float ReloadTime;
    public float AutoFireRate; // measured in bullets per second
    public int RicochetMaxBounce;
    [Range(0, 1)] public float CritChance;
    [Range(0, 1)] public float BullseyeChance;
    public float AerialStrikeChanceMaxAmmo;

    public void Reset()
    {
        Damage = 0f;
        MaxAmmo = 0;
        ReloadTime = 0f;
        AutoFireRate = 0f;
        RicochetMaxBounce = 0;
        CritChance = 0f;
        BullseyeChance = 0f;
        AerialStrikeChanceMaxAmmo = 0f;
    }
}
