using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseAerialStrikeChance", menuName = "Skill Tree/Effects/Increase Aerial Strike Chance")]
public class IncreaseAerialStrikeChance : SkillEffect
{
    [SerializeField, Range(0, 1)] private float aerialStrikeChance = 0.01f;
    [SerializeField] private PlayerStatsCondition variable;

    public override void Apply(SkillEffectContext context)
    {
        float chanceToAdd = 0f;
        if (variable == PlayerStatsCondition.None)
        {
            chanceToAdd = aerialStrikeChance;
        } else if (variable == PlayerStatsCondition.MaxAmmo)
        {
            chanceToAdd = aerialStrikeChance * context.PlayerRuntimeStats.MaxAmmo;
        }

        context.PlayerRuntimeStats.AerialStrikeChance += chanceToAdd;
    }
}
