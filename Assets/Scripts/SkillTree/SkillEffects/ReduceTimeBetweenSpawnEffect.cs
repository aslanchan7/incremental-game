using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Effects/Reduce Time Between Spawns")]
public class ReduceTimeBetweenSpawnEffect : SkillEffect {
    [SerializeField] private float amount;

    public override void Apply(SkillEffectContext context) {
        context.TargetSpawner.TimeBetweenSpawns -= amount;
        Debug.Log($"Reduced Time Between Spawns: {context.TargetSpawner.TimeBetweenSpawns}");
    }
}