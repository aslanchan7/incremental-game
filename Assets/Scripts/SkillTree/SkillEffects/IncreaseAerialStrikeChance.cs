using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseAerialStrikeChance", menuName = "Skill Tree/Effects/Increase Aerial Strike Chance")]
public class IncreaseAerialStrikeChance : SkillEffect
{
    [SerializeField, Range(0, 1)] private float aerialStrikeChance = 0.01f;
    [SerializeField] private PlayerStatsCondition variable;

    public override void Apply(SkillEffectContext context)
    {
        if (variable == PlayerStatsCondition.None)
        {
            context.PlayerRuntimeStats.AerialStrikeChance += aerialStrikeChance;
        } else if (variable == PlayerStatsCondition.MaxAmmo)
        {
            context.UpgradesData.AerialStrikeChanceMaxAmmo += aerialStrikeChance;
        }
    }
}
