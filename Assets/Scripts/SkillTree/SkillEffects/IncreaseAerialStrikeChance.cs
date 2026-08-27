using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseAerialStrikeChance", menuName = "Skill Tree/Effects/Increase Aerial Strike Chance")]
public class IncreaseAerialStrikeChance : SkillEffect
{
    [SerializeField, Range(0, 1)] private float aerialStrikeChance = 0.01f;

    public override void Apply(SkillEffectContext context)
    {
        context.PlayerRuntimeStats.AerialStrikeChance += aerialStrikeChance;
    }
}
