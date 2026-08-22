using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Effects/Increment Max Ammo")]
public class IncrementMaxAmmoEffect : SkillEffect {
    [SerializeField] private int amount;

    public override void Apply(SkillEffectContext context) {
        context.ShootingModule.MaxAmmo += amount;
        Debug.Log($"Upgraded Max Ammo: {context.ShootingModule.MaxAmmo}");
    }
}