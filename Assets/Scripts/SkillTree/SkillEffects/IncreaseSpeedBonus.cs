using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Increase Speed Bonus", menuName = "Skill Tree/Effects/Increase Speed Bonus")]
public class IncreaseSpeedBonus : SkillEffect
{
    [SerializeField] private float speedBonusCashIncrease = 5f;

    public override void Apply(SkillEffectContext context)
    {
        context.RoundRuntimeData.SpeedBonusCash += speedBonusCashIncrease;
    }
}
