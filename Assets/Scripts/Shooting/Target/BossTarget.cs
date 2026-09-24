using System.Collections;
using UnityEngine;

public class BossTarget : Target
{
    [Header("Boss Config")]
    [SerializeField] BossTimerBar bossTimerBar;
    [Space(10)]
    [SerializeField] private Vector3 initScale;
    [SerializeField] private Vector3 finalScale;
    [SerializeField] private float initSpeed;
    [SerializeField] private float finalSpeed;
    [SerializeField] private float timerDuration;
    [HideInInspector] public BossSpawner BossSpawner;

    private Vector2 moveDir;
    private float currSpeed;
    private CircleCollider2D collider;

    protected override void Awake()
    {
        base.Awake();

        currSpeed = initSpeed;
        collider = GetComponent<CircleCollider2D>();
        bossTimerBar = GetComponentInChildren<BossTimerBar>();
    }

    protected override void Start()
    {
        transform.localScale = initScale;
        currHealth = MaxHealth;

        // Random initial direction
        float[] diagonals = { 45f, 135f, 225f, 315f };
        float baseAngle = diagonals[Random.Range(0, 4)];
        float jitter = Random.Range(-15f, 15f);
        float angleDeg = baseAngle + jitter;

        float angleRad = angleDeg * Mathf.Deg2Rad;        
        moveDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;

        bossTimerBar.StartTimerBar(timerDuration);
    }

    public override void HandleShot(bool isBullseye, bool isCrit, bool isAerialStrike, Vector3 shotPos)
    {
        if (currHealth <= 0f)
        {
            return;
        }

        float damage = GameManager.Instance.CurrGunInstance.Damage;
        damage *= isCrit ? 2f : 1f;
        if (isAerialStrike)
            damage = 100;
        currHealth -= damage;
        if (currHealth < 0f) currHealth = 0f;

        RoundManager.Instance.TotalBullseyesHit += isBullseye ? 1 : 0;
        SFXManager.PlaySound(SoundType.TargetHit);
        Instantiate(targetHitParticles, shotPos, Quaternion.identity);
        // Instantiate(bulletHolePrefab, shotPos, Quaternion.identity, transform);
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
        // TargetSpawner.RemainingTargets--;
        BossSpawner.SpawnedBossTargets.Remove(this);
        RoundManager.Instance.TotalTargetsHit++;

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
        UpdatePosition();
    
        if (bossTimerBar.RemainingDuration <= 0f)
        {
            BossSpawner.SpawnedBossTargets.Remove(this);
            BossSpawner.outOfTime = true;
            Destroy(gameObject);
            Debug.Log("Failed to kill boss");
        }
    }

    private void UpdatePosition()
    {
        float radius = collider.radius * transform.lossyScale.x; // account for scaling
        Vector3 pos = transform.position;
        currSpeed = Mathf.Lerp(finalSpeed, initSpeed, currHealth/MaxHealth);

        // Move
        pos += (Vector3)(moveDir * Time.deltaTime) * currSpeed;

        // Compute effective bounds (inset by radius so the circle never clips outside)
        float minX = BossSpawner.SpawnBounds.bounds.min.x + radius;
        float maxX = BossSpawner.SpawnBounds.bounds.max.x - radius;
        float minY = BossSpawner.SpawnBounds.bounds.min.y + radius;
        float maxY = BossSpawner.SpawnBounds.bounds.max.y - radius;

        // Bounce off X walls
        if (pos.x < minX)
        {
            pos.x = minX;
            moveDir.x = -moveDir.x;
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            moveDir.x = -moveDir.x;
        }

        // Bounce off Y walls
        if (pos.y < minY)
        {
            pos.y = minY;
            moveDir.y = -moveDir.y;
        }
        else if (pos.y > maxY)
        {
            pos.y = maxY;
            moveDir.y = -moveDir.y;
        }

        transform.position = pos;
    }
}
