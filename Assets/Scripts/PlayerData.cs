using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public int MaxAmmo;
    public float ReloadTime;
    public Color ActiveCrosshairColor;
    public Color InactiveCrosshairColor;
    [Range(0, 1)] public float RicochetShotChance;
    public int RicochetMaxBounce = 1;
    [Range(0, 1)] public float RicochetBullseyeChance;
    [Range(0, 1)] public float AerialStrikeChance;
}
