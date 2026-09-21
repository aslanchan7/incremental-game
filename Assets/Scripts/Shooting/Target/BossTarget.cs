using System.Collections;
using UnityEngine;

public class BossTarget : Target
{
    [Header("Boss Config")]
    [SerializeField] private Vector3 initScale;
    [SerializeField] private Vector3 finalScale;

    protected override void Start()
    {
        healthBar = Instantiate(healthBarPrefab, transform);
        Vector3 worldPos = transform.position + healthBarOffsetWorldSpace;
        healthBar.SetPositionWorldSpace(worldPos);
        transform.localScale = initScale;

        currHealth = MaxHealth;
    }

    public override void HandleShot(bool isBullseye, bool isCrit, bool isAerialStrike, Vector3 shotPos)
    {
        float damage = GameManager.Instance.CurrGunInstance.Damage;
        damage *= isCrit ? 2f : 1f;
        if (isAerialStrike)
            damage = 100;
        currHealth -= damage;
        if (currHealth < 0f) currHealth = 0f;

        SFXManager.PlaySound(SoundType.TargetHit);
        Instantiate(targetHitParticles, shotPos, Quaternion.identity);
        HandleFlytext(isBullseye, isCrit);

        StartCoroutine(healthBar.UpdateHealthBar(currHealth, MaxHealth, healthBarAnimTime));
        StartCoroutine(UpdateScale(currHealth, MaxHealth, healthBarAnimTime));

        if (currHealth <= 0f)
        {
            StartCoroutine(TargetDestroyed(isBullseye));
        }
    }

    protected override IEnumerator TargetDestroyed(bool isBullseye)
    {
        // TargetSpawner.SpawnedTargets.Remove(this);
        // TargetSpawner.RemainingTargets--;
        RoundManager.Instance.TotalTargetsHit++;
        RoundManager.Instance.TotalBullseyesHit += isBullseye ? 1 : 0;

        AddMoneyEarned(isBullseye);

        do
        {
            yield return null;
        } while (healthBar.IsAnimating);
        Destroy(gameObject);
    }

    protected override void HandleFlytext(bool isBullseye, bool isCrit)
    {
        if (isBullseye)
        {
            Flytext flytext = Instantiate(flytextPrefab, transform.position, Quaternion.identity);
            flytext.Show("bullseye!", 1f, Vector2.up, bullseyeFlytextColor);
        }

        if (isCrit)
        {
            Flytext flytext = Instantiate(flytextPrefab, transform.position, Quaternion.identity);
            flytext.Show("critical!", 1f, Vector2.up, bullseyeFlytextColor);
        }
    }

    protected override void AddMoneyEarned(bool isBullseye)
    {
        RoundRuntimeData roundRuntimeData = GameManager.Instance.RoundRuntimeData;
        double moneyEarned = isBullseye ? BaseValue * roundRuntimeData.BullseyeMultiplier : BaseValue;
        // Account for Combo Bonus
        // float comboBonusMult = 1 + (TargetSpawner.CurrCombo * TargetSpawner.comboMultPerHit);
        // moneyEarned = roundRuntimeData.IsComboBonusActive ? moneyEarned * comboBonusMult : moneyEarned;
        RoundManager.Instance.TotalMoneyEarned += moneyEarned;

        CurrencyManager.Instance.Add("cash", moneyEarned);
    }

    protected override void HandleRespawnTarget()
    {
        return;
    }

    private IEnumerator UpdateScale(float currHealth, float MaxHealth, float animTime)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.Lerp(finalScale, initScale, currHealth/MaxHealth);

        yield return BasicAnimations.Interpolate(
            () =>
            {
                transform.localScale = startScale;
            },
            (t) =>
            {
                transform.localScale = Vector3.Lerp(startScale, endScale, t);
            },
            () =>
            {
                transform.localScale = endScale;
            },
            animTime
        );
    }

    void Update()
    {
        // Bounce off the walls
    }
}
