using System;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    [Header("References")]

    [Header("Skill Tree")]
    public SkillTreeNode TreeHead;
    [SerializeField] private Transform nodesParent;
    private SkillEffectContext context;

    [Header("Actions")]
    public Action<SkillTreeNode> OnNodePurchased;
    public Action OnNodeDataInitialized;

    void Start()
    {
        context = new(GameManager.Instance.PlayerRuntimeStats, GameManager.Instance.RoundRuntimeData);

        InitializeNodesData();

        // TODO: REMOVE THIS
        // CurrencyManager.Instance.Add("cash", 1000);
    }

    void InitializeNodesData()
    {
        for (int i = 0; i < nodesParent.childCount; i++)
        {
            SkillTreeNode node = nodesParent.GetChild(i).GetComponent<SkillTreeNode>();
            bool isPurchased = GameManager.Instance.SkillTree.PurchasedNodeIds.Contains(node.Data.id);
            node.IsPurchased = isPurchased;
        }
        OnNodeDataInitialized?.Invoke();
    }

    public void TryPurchase(SkillTreeNode node)
    {
        if (!node.IsUnlocked) // can't purchased locked node 
        {
            Debug.Log($"{node.Data.displayName} has not been unlocked yet");
            return;
        }

        if (node.IsPurchased) // if node is already purchased then do nothing
        {
            Debug.Log($"{node.Data.displayName} has already been purchased");
            return;
        }

        if (CurrencyManager.Instance.GetCurrency("cash").amount >= node.Data.cost)
        {
            bool moneySpent = CurrencyManager.Instance.TrySpend("cash", node.Data.cost);
            if (!moneySpent) // if failed to spend money then don't apply upgrades. moneySpent should always be True but this is just a failsafe
            {
                Debug.LogWarning("Failed to spend money");
                return;
            }

            node.ApplyEffects(context);
            node.IsPurchased = true;
            GameManager.Instance.SkillTree.PurchasedNodeIds.Add(node.Data.id);
            OnNodePurchased?.Invoke(node);
            Debug.Log($"Purchased {node.Data.displayName}");
        }
        else
        {
            Debug.Log("Not Enough Money");
        }
    }
}
