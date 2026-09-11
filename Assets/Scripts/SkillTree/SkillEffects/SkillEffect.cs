using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    public abstract void Apply(SkillEffectContext context);
}

public class SkillEffectContext {
    public PlayerRuntimeStats PlayerRuntimeStats;
    public RoundRuntimeData RoundRuntimeData;
    public UpgradesData UpgradesData;

    public SkillEffectContext(PlayerRuntimeStats playerRuntimeStats, RoundRuntimeData roundRuntimeData, UpgradesData upgradesData)
    {
        PlayerRuntimeStats = playerRuntimeStats;
        RoundRuntimeData = roundRuntimeData;
        UpgradesData = upgradesData;
    }
}
