using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DecreaseReloadTime", menuName = "Skill Tree/Effects/Decrease Reload Time")]
public class DecreaseReloadTime : SkillEffect
{
    // [SerializeField] private float decreaseTime = 0.5f;
    [SerializeField] private float decreasePercentage = 0.25f;
    public override void Apply(SkillEffectContext context)
    {
        float decreaseTime = decreasePercentage * context.PlayerRuntimeStats.ReloadTime;
        context.PlayerRuntimeStats.ReloadTime -= decreaseTime;
    }
}
