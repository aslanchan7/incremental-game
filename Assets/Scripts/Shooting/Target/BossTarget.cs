using System.Collections;
using UnityEngine;

public class BossTarget : Target
{
    protected override void Start()
    {

    }

    public override void HandleShot(bool isBullseye, bool isCrit, bool isAerialStrike, Vector3 shotPos)
    {

    }

    protected override IEnumerator TargetDestroyed(bool isBullseye)
    {
        yield return null;
    }

    protected override void HandleFlytext(bool isBullseye, bool isCrit)
    {

    }

    protected override void AddMoneyEarned(bool isBullseye)
    {

    }

    protected override void HandleRespawnTarget()
    {

    }

}
