using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    public override IEnumerator Shoot(Vector3 tracerStartPos, Vector3 tracerEndPos, bool isAerialStrike, Target hit, bool isBullseye, bool isRicochet)
    {
        if (isRicochet)
        {
            yield return SpawnTracer(tracerStartPos, tracerEndPos, isAerialStrike);
        } else
        {
            for (int i = 0; i < 4; i++)
            {
                StartCoroutine(SpawnTracer(tracerStartPos, tracerEndPos + GetRandomVector3(), isAerialStrike));
            }
            yield return SpawnTracer(tracerStartPos, tracerEndPos + GetRandomVector3(), isAerialStrike);
        }

        bool isCrit = critChanceBag.Pull(CritChance) && !isAerialStrike;

        ShotFired?.Invoke(hit, isBullseye, isCrit, isAerialStrike, tracerEndPos);
    }

    private Vector3 GetRandomVector3()
    {
        float x = Random.Range(-0.9f, 0.9f);
        float y = Random.Range(-0.9f, 0.9f);
        return new Vector3(x, y, 0f);
    }
}
