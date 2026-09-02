using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GunData", menuName = "Gun Data Scriptable Object")]
public class GunDataSO : ScriptableObject
{
    public int MaxAmmo;
    public float ReloadTime;
    public float AutoFireRate; // measured in bullets per second
    [Range(0, 1)] public float BullseyeChance;
    // public float ChanceMult;
}
