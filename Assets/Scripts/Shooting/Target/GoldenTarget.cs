using System.Collections;
using UnityEngine;

public class GoldenTarget : Target
{
    [Header("Golden Target Settings")]
    [SerializeField] private float selfDestroyTime = 2.5f;
    [SerializeField] private float cashMult = 10f;
    [SerializeField] private Color goldenFlytextColor;

    void Awake()
    {
        BaseValue = GameManager.Instance.RoundRuntimeData.BaseTargetValue * cashMult;
    }

    void Start()
    {
        StartCoroutine(GoldenTargetSelfDestroy());
    }

    private IEnumerator GoldenTargetSelfDestroy()
    {
        yield return new WaitForSeconds(selfDestroyTime);
        Destroy(gameObject);
        TargetSpawner.RemainingTargets--;
        Debug.Log("Golden Target destroyed itself");
    }

    protected override void HandleFlytext(bool isBullseye)
    {
        base.HandleFlytext(isBullseye);

        if (currHealth <= 0f)
        {
            Flytext flytext = Instantiate(flytextPrefab, transform.position, Quaternion.identity);
            flytext.Show("x10", 1f, Vector2.up, goldenFlytextColor);            
        }
    }
}
