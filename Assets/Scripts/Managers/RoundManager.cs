using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private List<Transform> roundIndicators;

    [Header("Round Info")]
    private bool isBossRound = false;

    void Start()
    {
        GameManager.Instance.CurrentRound++;
        isBossRound = GameManager.Instance.CurrentRound % 10 == 0;

        UpdateUI();

        if (isBossRound)
        {
            targetSpawner.gameObject.SetActive(false);
            StartBossRound();
        } else
        {
            targetSpawner.gameObject.SetActive(true);
            targetSpawner.StartRound();
        }
    }

    void StartBossRound()
    {
        // Instantiate(bossPrefab);
    }

    //  -------------- UI STUFF ---------------
    void UpdateUI()
    {
        for (int i = 0; i < roundIndicators.Count; i++)
        {
            if (i == (GameManager.Instance.CurrentRound % 10) - 1)
            {
                roundIndicators[i].localScale *= 1.25f;
            } else if (i < (GameManager.Instance.CurrentRound % 10) - 1)
            {
                roundIndicators[i].GetComponent<Image>().color *= Color.gray;
            }
        }
    }

    
}
