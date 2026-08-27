using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseRicochetMaxBounce", menuName = "Skill Tree/Effects/Increase Ricochet Max Bounce")]
public class IncreaseRicochetMaxBounce : SkillEffect
{
    [SerializeField] private int count;
    public override void Apply(SkillEffectContext context)
    {
        context.PlayerRuntimeStats.RicochetMaxBounce += count;
    }
}
