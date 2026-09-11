using UnityEngine;

public class Sniper : Gun
{
    protected override void CheckForUpgrades()
    {
        base.CheckForUpgrades();

        GameManager.Instance.PlayerRuntimeStats.AerialStrikeChance *= 2f;
        GameManager.Instance.PlayerRuntimeStats.RicochetBullseyeChance *= 2f;
        GameManager.Instance.RoundRuntimeData.GoldenTargetChance *= 2f;
        GameManager.Instance.RoundRuntimeData.TargetRespawnChance *= 2f;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        GameManager.Instance.PlayerRuntimeStats.AerialStrikeChance /= 2f;
        GameManager.Instance.PlayerRuntimeStats.RicochetBullseyeChance /= 2f;
        GameManager.Instance.RoundRuntimeData.GoldenTargetChance /= 2f;
        GameManager.Instance.RoundRuntimeData.TargetRespawnChance /= 2f;
    }

}
