using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    public abstract void Apply(SkillEffectContext context);
}

public class SkillEffectContext {
    public ShootingModule ShootingModule;
    public TargetSpawner TargetSpawner;

    public SkillEffectContext(ShootingModule shootingModule, TargetSpawner targetSpawner)
    {
        ShootingModule = shootingModule;
        TargetSpawner = targetSpawner;
    }
}
