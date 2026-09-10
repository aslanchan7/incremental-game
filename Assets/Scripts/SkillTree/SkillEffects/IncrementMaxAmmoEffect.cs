using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Skill Tree/Effects/Increment Max Ammo")]
public class IncrementMaxAmmoEffect : SkillEffect {
    [SerializeField] private int amount;

    public override void Apply(SkillEffectContext context) {
        // TODO
        // context.GunData.MaxAmmo += amount;
    }
}