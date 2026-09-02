using UnityEngine;

public class PlayerRuntimeStats
{
    public int MaxAmmo;
    public float ReloadTime;
    public float AutoFireRate;
    public Color ActiveCrosshairColor;
    public Color InactiveCrosshairColor;
    public float BullseyeChance;
    public float RicochetShotChance;
    public int RicochetMaxBounce;
    public float RicochetBullseyeChance;
    public float AerialStrikeChance;

    public PlayerRuntimeStats(PlayerData playerData, GunDataSO gunData)
    {
        ActiveCrosshairColor = playerData.ActiveCrosshairColor;
        InactiveCrosshairColor = playerData.InactiveCrosshairColor;
        RicochetMaxBounce = playerData.RicochetMaxBounce;
        RicochetShotChance = playerData.RicochetShotChance;
        RicochetBullseyeChance = playerData.RicochetBullseyeChance;
        AerialStrikeChance = playerData.AerialStrikeChance;

        MaxAmmo = gunData.MaxAmmo;
        ReloadTime = gunData.ReloadTime;
        AutoFireRate = gunData.AutoFireRate;
        BullseyeChance = gunData.BullseyeChance;
    }
}

public enum PlayerStatsCondition
{
    None = 0,
    MaxAmmo = 1 << 0,
    // TODO: ADD MORE CONDITION FLAGS AS NECESSARY
}