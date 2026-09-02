using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseBullseyeChance", menuName = "Skill Tree/Effects/Increase Bullseye Chance")]
public class IncreaseBullseyeChance : SkillEffect
{
    [SerializeField, Range(0, 1)] private float bullseyeChance;
    public override void Apply(SkillEffectContext context)
    {
        context.PlayerRuntimeStats.BullseyeChance += bullseyeChance;
    }
}