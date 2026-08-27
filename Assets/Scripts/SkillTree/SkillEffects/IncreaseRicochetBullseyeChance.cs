using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseRicochetBullseyeChance", menuName = "Skill Tree/Effects/Increase Ricochet Bullseye Chance")]
public class IncreaseRicochetBullseyeChance : SkillEffect
{
    [SerializeField, Range(0, 1)] private float chanceToAdd = 0.25f;
    public override void Apply(SkillEffectContext context)
    {
        context.PlayerRuntimeStats.RicochetBullseyeChance += chanceToAdd;
    }
}
