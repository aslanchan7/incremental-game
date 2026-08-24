using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    public abstract void Apply(SkillEffectContext context);
}

public class SkillEffectContext {
    public PlayerRuntimeStats PlayerRuntimeStats;
    public RoundRuntimeData RoundRuntimeData;

    public SkillEffectContext(PlayerRuntimeStats playerRuntimeStats, RoundRuntimeData roundRuntimeData)
    {
        PlayerRuntimeStats = playerRuntimeStats;
        RoundRuntimeData = roundRuntimeData;
    }
}
