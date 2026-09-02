using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "RoundData", menuName = "Data/Round Data")]
public class RoundData : ScriptableObject
{
    public int InitialTargetCount; // number of targets to spawn at the start of the run
    // public int TotalTargetCount; // number of targets to spawn in total (including spawning over time)
    public float TimeBetweenSpawns; // after initial spawning, targets will spawn with this amount of time delay
    [Range(0, 1)] public float TargetRespawnChance;
    public float BaseTargetValue;
    public float BullseyeMultiplier; 
    public float SpeedBonusCash;
}
