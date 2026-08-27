using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseBullseyeMultiplier", menuName = "Skill Tree/Effects/Increase Bullseye Multiplier")]
public class IncreaseBullseyeMultiplier : SkillEffect
{
    [SerializeField] private float increaseVal = 1f;

    public override void Apply(SkillEffectContext context)
    {
        context.RoundRuntimeData.BullseyeMultiplier += increaseVal;    
    }
}
