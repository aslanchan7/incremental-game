using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "RoundData", menuName = "Data/Round Data")]
public class RoundData : ScriptableObject
{
    public int TargetCount; // number of targets we WANT to spawn in total
    public float TimeBetweenSpawns; // after initial spawning, targets will spawn with this amount of time delay
    public float BaseTargetValue;
    public float BullseyeMultiplier; 
}
