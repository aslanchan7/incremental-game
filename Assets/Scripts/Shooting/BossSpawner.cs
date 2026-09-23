using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public BoxCollider2D SpawnBounds;
    [SerializeField] private BossTarget bossTargetPrefab;
    public List<BossTarget> SpawnedBossTargets = new();
    public void StartRound()
    {
        // TODO: BOSS INTRO ANIM
        BossTarget boss = Instantiate(bossTargetPrefab);
        boss.BossSpawner = this;
        SpawnedBossTargets.Add(boss);
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
        if (SpawnedBossTargets.Count == 0)
        {
            RoundManager.Instance.EndBossRound();
        }
    }
}
