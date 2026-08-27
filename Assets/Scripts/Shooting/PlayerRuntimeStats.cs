using UnityEngine;

public class PlayerRuntimeStats
{
    public int MaxAmmo;
    public float ReloadTime;
    public Color ActiveCrosshairColor;
    public Color InactiveCrosshairColor;
    public float RicochetShotChance;
    public int RicochetMaxBounce;
    public float RicochetBullseyeChance;
    public float AerialStrikeChance;

    public PlayerRuntimeStats(PlayerData playerData)
    {
        MaxAmmo = playerData.MaxAmmo;
        ReloadTime = playerData.ReloadTime;
        ActiveCrosshairColor = playerData.ActiveCrosshairColor;
        InactiveCrosshairColor = playerData.InactiveCrosshairColor;
        RicochetShotChance = playerData.RicochetShotChance;
        RicochetMaxBounce = playerData.RicochetMaxBounce;
        RicochetBullseyeChance = playerData.RicochetBullseyeChance;
        AerialStrikeChance = playerData.AerialStrikeChance;
    }
}
