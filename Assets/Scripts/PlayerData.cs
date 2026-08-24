using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public int MaxAmmo;
    public float ReloadTime;
    public Color ActiveCrosshairColor;
    public Color InactiveCrosshairColor;
}
