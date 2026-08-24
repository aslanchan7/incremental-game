using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Skill Tree/Effects/Ricochet Shot Chance")]
public class RicochetShotChanceEffect : SkillEffect {
    [SerializeField, Range(0, 1)] private float chance;

    public override void Apply(SkillEffectContext context) {
        // context.PlayerData.RegisterMultiShotChance(chance);
        Debug.LogWarning("Not Implemented");
    }
}