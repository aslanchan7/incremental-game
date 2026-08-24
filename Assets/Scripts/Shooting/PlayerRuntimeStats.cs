using UnityEngine;

public class PlayerRuntimeStats
{
    public int MaxAmmo;
    public float ReloadTime;
    public Color ActiveCrosshairColor;
    public Color InactiveCrosshairColor;

    public PlayerRuntimeStats(PlayerData playerData)
    {
        MaxAmmo = playerData.MaxAmmo;
        ReloadTime = playerData.ReloadTime;
        ActiveCrosshairColor = playerData.ActiveCrosshairColor;
        InactiveCrosshairColor = playerData.InactiveCrosshairColor;
    }
}
