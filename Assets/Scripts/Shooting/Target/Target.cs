using System.Collections;
using BreakInfinity;
using UnityEngine;

public abstract class Target : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public TargetSpawner TargetSpawner;
    [SerializeField] protected GameObject targetHitParticles;
    
    [Header("Target Variables")]
    public float BaseValue;
    public float MaxHealth;
    [SerializeField] protected float currHealth;

    [Header("Flytext Settings")]
    [SerializeField] protected Flytext flytextPrefab;
    [SerializeField] protected Color bullseyeFlytextColor;

    [Header("Health Bar Settings")]
    [SerializeField] protected HealthBarUI healthBarPrefab;
    protected HealthBarUI healthBar;
    [SerializeField] protected Vector3 healthBarOffsetWorldSpace;
    [SerializeField] protected float healthBarAnimTime = 0.1f;

    protected virtual void Start()
    {
        healthBar = Instantiate(healthBarPrefab, transform);
        Vector3 worldPos = transform.position + healthBarOffsetWorldSpace;
        healthBar.SetPositionWorldSpace(worldPos);
        BaseValue = GameManager.Instance.RoundRuntimeData.BaseTargetValue;

        currHealth = MaxHealth;
    }

    // HANDLE THEIR OWN HEALTH
    // HANDLE DESTROYING THEMSELVES -- INCLUDING GIVING MONEY, INCREMENTING COMBO, PLAY SFX, PARTICLES, ETC
    public virtual void HandleShot(bool isBullseye, float damage, Vector3 shotPos)
    {
        currHealth -= damage;

        SFXManager.PlaySound(SoundType.TargetHit);
        Instantiate(targetHitParticles, shotPos, Quaternion.identity);
        HandleFlytext(isBullseye);

        StartCoroutine(healthBar.UpdateHealthBar(currHealth, MaxHealth, healthBarAnimTime));

        if (currHealth <= 0f)
        {
            StartCoroutine(TargetDestroyed(isBullseye));
        }
    }

    protected virtual IEnumerator TargetDestroyed(bool isBullseye)
    {
        TargetSpawner.SpawnedTargets.Remove(this);
        TargetSpawner.RemainingTargets--;
        TargetSpawner.TotalTargetsHit++;
        TargetSpawner.TotalBullseyesHit += isBullseye ? 1 : 0;

        AddMoneyEarned(isBullseye);
        HandleRespawnTarget();

        do
        {
            yield return null;
        } while (healthBar.IsAnimating);
        Destroy(gameObject);
    }

    protected virtual void HandleFlytext(bool isBullseye)
    {
        if (isBullseye)
        {
            Flytext flytext = Instantiate(flytextPrefab, transform.position, Quaternion.identity);
            flytext.Show("bullseye!", 1f, Vector2.up, bullseyeFlytextColor);
        }
    }

    protected virtual void AddMoneyEarned(bool isBullseye)
    {
        RoundRuntimeData roundRuntimeData = GameManager.Instance.RoundRuntimeData;
        BigDouble moneyEarned = isBullseye ? BaseValue * roundRuntimeData.BullseyeMultiplier : BaseValue;
        // Account for Combo Bonus
        float comboBonusMult = 1 + (TargetSpawner.CurrCombo * TargetSpawner.comboMultPerHit);
        moneyEarned = roundRuntimeData.IsComboBonusActive ? moneyEarned * comboBonusMult : moneyEarned;
        TargetSpawner.TotalMoneyEarned += moneyEarned;

        CurrencyManager.Instance.Add("cash", moneyEarned);
    }

    protected virtual void HandleRespawnTarget()
    {
        // Potentially respawn target here based on TargetRespawnChance
        RoundRuntimeData roundRuntimeData = GameManager.Instance.RoundRuntimeData;
        if (roundRuntimeData.TargetRespawnChance != 0f)
        {
            bool respawnTarget = TargetSpawner.TargetRespawnChanceBag.Pull(roundRuntimeData.TargetRespawnChance);
            if (respawnTarget)
            {
                bool isGoldenTarget = false;
                if (roundRuntimeData.GoldenTargetChance != 0f)
                {
                    isGoldenTarget = TargetSpawner.GoldenTargetChanceBag.Pull(roundRuntimeData.GoldenTargetChance);
                }

                int segmentIdx = Random.Range(0, TargetSpawner.ScreenSegments);
                TargetSpawner.SpawnTarget(segmentIdx, isGoldenTarget);
            }
        }   
    }
}
