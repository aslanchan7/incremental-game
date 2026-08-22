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
        } else
        {
            Instance = this;
        }

        skillTree = new(treeHead);
    }

    [Header("References")]
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private ShootingModule shootingModule;
    [SerializeField] private TargetSpawner targetSpawner;

    [Header("Skill Tree")]
    [SerializeField] private SkillTreeNode treeHead;
    private SkillTree skillTree;
    private SkillEffectContext context;

    [Header("Actions")]
    public Action<SkillTreeNode> OnNodePurchased;

    void Start()
    {
        context = new(shootingModule, targetSpawner);
    }

    public void TryPurchase(SkillTreeNode node)
    {
        if (!node.IsUnlocked) return; // can't purchased locked node

        if (node.IsPurchased) return; // if node is already purchased then do nothing

        if (currencyManager.GetCurrency("cash").amount >= node.Data.cost)
        {
            node.ApplyEffects(context);
            node.IsPurchased = true;
            OnNodePurchased?.Invoke(node);
            Debug.Log($"Purchased {node.Data.displayName}");
        } else
        {
            Debug.Log("Not Enough Money");
        }
    }
}
