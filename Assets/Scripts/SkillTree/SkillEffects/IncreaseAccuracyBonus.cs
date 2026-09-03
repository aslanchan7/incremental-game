using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseAccuracyBonus", menuName = "Skill Tree/Effects/Increase Accuracy Bonus")]
public class IncreaseAccuracyBonus : SkillEffect
{
    [SerializeField] private float accuracyBonusIncrease = 0.1f;
    public override void Apply(SkillEffectContext context)
    {
        context.RoundRuntimeData.AccuracyBonusCashPercentage = accuracyBonusIncrease;
    }
}
