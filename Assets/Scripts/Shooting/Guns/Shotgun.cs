using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    protected float bulletSpread = 0.4f;
    protected float numOfBullets = 4f;

    public override IEnumerator Shoot(Vector3 tracerStartPos, Vector3 tracerEndPos, bool isAerialStrike, Target hit, bool isBullseye, bool isRicochet)
    {
        if (isRicochet)
        {
            yield return SpawnTracer(tracerStartPos, tracerEndPos, isAerialStrike);
        } else
        {
            for (int i = 0; i < numOfBullets; i++)
            {
                bool isCrit;

                // This bullet will go where the player aimed
                if (i == 0)
                {
                    StartCoroutine(SpawnTracer(tracerStartPos, tracerEndPos, isAerialStrike));

                    isCrit = critChanceBag.Pull(CritChance) && !isAerialStrike;
                    ShotFired?.Invoke(hit, isBullseye, isCrit, isAerialStrike, tracerEndPos);

                    continue;
                }
                
                StartCoroutine(SpawnTracer(tracerStartPos, tracerEndPos + GetRandomVector3(), isAerialStrike));
            
                isCrit = critChanceBag.Pull(CritChance) && !isAerialStrike;
                ShotFired?.Invoke(hit, false, isCrit, isAerialStrike, tracerEndPos + GetRandomVector3());

                // This last bullet will call 'yield return' to wait for the bullet to travel to the target
                if (i == numOfBullets - 1)
                {
                    yield return SpawnTracer(tracerStartPos, tracerEndPos + GetRandomVector3(), isAerialStrike);
                
                    isCrit = critChanceBag.Pull(CritChance) && !isAerialStrike;
                    ShotFired?.Invoke(hit, false, isCrit, isAerialStrike, tracerEndPos + GetRandomVector3());                    

                    continue;
                }
            }
        }

        // bool isCrit = critChanceBag.Pull(CritChance) && !isAerialStrike;
        // ShotFired?.Invoke(hit, isBullseye, isCrit, isAerialStrike, tracerEndPos);
    }

    private Vector3 GetRandomVector3()
    {
        float x = Random.Range(-bulletSpread, bulletSpread);
        float y = Random.Range(-bulletSpread, bulletSpread);
        return new Vector3(x, y, 0f);
    }
}
