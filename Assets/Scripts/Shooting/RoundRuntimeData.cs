using UnityEngine;

public class RoundRuntimeData
{
    public int TargetCount;
    public float TimeBetweenSpawns;
    public float BaseTargetValue;
    public float BullseyeMultiplier; 
    public float SpeedBonusCash;

    public RoundRuntimeData(RoundData roundData)
    {
        TargetCount = roundData.TargetCount;
        TimeBetweenSpawns = roundData.TimeBetweenSpawns;
        BaseTargetValue = roundData.BaseTargetValue;
        BullseyeMultiplier = roundData.BullseyeMultiplier;
        SpeedBonusCash = roundData.SpeedBonusCash;
    }
}
