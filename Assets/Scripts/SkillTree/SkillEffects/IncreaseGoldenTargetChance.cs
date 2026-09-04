using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseGoldenTargetChance", menuName = "Skill Tree/Effects/Increase Golden Target Chance")]
public class IncreaseGoldenTargetChance : SkillEffect
{
    [SerializeField, Range(0, 1)] private float chanceToAdd;
    public override void Apply(SkillEffectContext context)
    {
        context.RoundRuntimeData.GoldenTargetChance += chanceToAdd;
    }
}