using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseTargetCount", menuName = "Skill Tree/Effects/Increase Target Count")]
public class IncreaseTargetCount : SkillEffect
{
    [SerializeField] private int count;
    public override void Apply(SkillEffectContext context)
    {
        context.RoundRuntimeData.InitialTargetCount += count;
        // context.RoundRuntimeData.TotalTargetCount += count; // If we increment initial, we should also increment total
    }
}
