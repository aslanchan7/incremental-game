using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public Color ActiveCrosshairColor;
    public Color InactiveCrosshairColor;
    [Range(0, 1)] public float RicochetShotChance;
    [Range(0, 1)] public float RicochetBullseyeChance;
    [Range(0, 1)] public float AerialStrikeChance;
}
