using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Skill Tree/Effects/Increment Max Ammo")]
public class IncrementMaxAmmoEffect : SkillEffect {
    [SerializeField] private int amount;

    public override void Apply(SkillEffectContext context) {
        context.PlayerRuntimeStats.MaxAmmo += amount;
        Debug.Log($"Upgraded Max Ammo: {context.PlayerRuntimeStats.MaxAmmo}");
    }
}