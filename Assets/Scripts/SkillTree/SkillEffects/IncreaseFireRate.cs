using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IncreaseFireRate", menuName = "Skill Tree/Effects/Increase Fire Rate")]
public class IncreaseFireRate : SkillEffect
{
    [SerializeField, Tooltip("Ex: 0.20f is a 20% INCREASE")] private float increaseRate;
    public override void Apply(SkillEffectContext context)
    {
        context.PlayerRuntimeStats.AutoFireRate *= increaseRate + 1; // +1 is to make sure the rate is increasing and not being divided
    }
}
