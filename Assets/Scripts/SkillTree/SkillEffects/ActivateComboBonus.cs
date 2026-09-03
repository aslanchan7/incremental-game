using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ActivateComboBonus", menuName = "Skill Tree/Effects/Activate Combo Bonus")]
public class ActivateComboBonus : SkillEffect
{
    public override void Apply(SkillEffectContext context)
    {
        context.RoundRuntimeData.IsComboBonusActive = true;
    }
}
