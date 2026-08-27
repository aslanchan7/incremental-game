using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseTargetValue", menuName = "Skill Tree/Effects/Increase Target Value")]
public class IncreaseTargetValue : SkillEffect
{
    [SerializeField] private int targetValIncrease = 1;

    public override void Apply(SkillEffectContext context)
    {
        context.RoundRuntimeData.BaseTargetValue += targetValIncrease;
    }
}
