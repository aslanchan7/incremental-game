using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private BossTarget bossTargetPrefab;
    public void StartRound()
    {
        // TODO: BOSS INTRO ANIM
        Instantiate(bossTargetPrefab);
    }

    void OnEnable()
    {
        Gun.ShotFired += HandleShotFired;
    }

    void OnDisable()
    {
        Gun.ShotFired -= HandleShotFired;
    }

    private void HandleShotFired(Target target, bool isBullseye, bool isCrit, bool isAerialStrike, Vector3 shotPos)
    {
        RoundManager.Instance.TotalShotsFired++;
        RoundManager.Instance.TotalShotsMissed += target == null ? 1 : 0;

        if (target != null)
        {
            target.HandleShot(isBullseye, isCrit, isAerialStrike, shotPos);
        }

        CheckRoundEndCondition();
    }

    void CheckRoundEndCondition()
    {

    }
}
