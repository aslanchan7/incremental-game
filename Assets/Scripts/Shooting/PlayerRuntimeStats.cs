using System;
using UnityEngine;

[Serializable]
public class PlayerRuntimeStats
{
    // public int MaxAmmo;
    // public float ReloadTime;
    // public float AutoFireRate;
    // public float BullseyeChance;
    // public float CritChance;
    public Color ActiveCrosshairColor;
    public Color InactiveCrosshairColor;
    public float RicochetShotChance;
    public float RicochetBullseyeChance;
    public float AerialStrikeChance;

    public PlayerRuntimeStats(PlayerData playerData)
    {
        ActiveCrosshairColor = playerData.ActiveCrosshairColor;
        InactiveCrosshairColor = playerData.InactiveCrosshairColor;
        RicochetShotChance = playerData.RicochetShotChance;
        RicochetBullseyeChance = playerData.RicochetBullseyeChance;
        AerialStrikeChance = playerData.AerialStrikeChance;

        // MaxAmmo = gunData.MaxAmmo;
        // ReloadTime = gunData.ReloadTime;
        // AutoFireRate = gunData.AutoFireRate;
        // BullseyeChance = gunData.BullseyeChance;
        // CritChance = gunData.CritChance;
    }
}

public enum PlayerStatsCondition
{
    None = 0,
    MaxAmmo = 1 << 0,
    // TODO: ADD MORE CONDITION FLAGS AS NECESSARY
}