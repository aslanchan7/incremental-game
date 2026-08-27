using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseTargetRespawnChance", menuName = "Skill Tree/Effects/Increase Target Respawn Chance")]
public class IncreaseTargetRespawnChance : SkillEffect
{
    [SerializeField, Range(0, 1)] private float respawnChance = 0.1f;

    public override void Apply(SkillEffectContext context)
    {
        context.RoundRuntimeData.TargetRespawnChance += respawnChance;
    }
}
