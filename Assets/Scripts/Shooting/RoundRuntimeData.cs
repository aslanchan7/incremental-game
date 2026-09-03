using UnityEngine;

public class RoundRuntimeData
{
    public int InitialTargetCount;
    // public int TotalTargetCount;
    public float TimeBetweenSpawns;
    public float TargetRespawnChance;
    public float BaseTargetValue;
    public float BullseyeMultiplier; 
    public float SpeedBonusCash;
    public float AccuracyBonusCashPercentage;
    public bool IsComboBonusActive;

    public RoundRuntimeData(RoundData roundData)
    {
        // if (TotalTargetCount < InitialTargetCount) Debug.LogWarning($"TotalTargetCount ({TotalTargetCount}) is less than InitialTargetCount ({InitialTargetCount}).");

        InitialTargetCount = roundData.InitialTargetCount;
        // TotalTargetCount = roundData.TotalTargetCount;
        TimeBetweenSpawns = roundData.TimeBetweenSpawns;
        TargetRespawnChance = roundData.TargetRespawnChance;
        BaseTargetValue = roundData.BaseTargetValue;
        BullseyeMultiplier = roundData.BullseyeMultiplier;
        SpeedBonusCash = roundData.SpeedBonusCash;
        IsComboBonusActive = roundData.IsComboBonusActive;
        AccuracyBonusCashPercentage = roundData.AccuracyBonusCashPercentage;
    }
}
