using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DecreaseReloadTime", menuName = "Skill Tree/Effects/Decrease Reload Time")]
public class DecreaseReloadTime : SkillEffect
{
    [SerializeField] private float decreaseTime = 0.5f;
    public override void Apply(SkillEffectContext context)
    {
        context.PlayerRuntimeStats.ReloadTime -= decreaseTime;
    }
}
