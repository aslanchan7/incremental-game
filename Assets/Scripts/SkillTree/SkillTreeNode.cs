using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkillTreeNode : MonoBehaviour
{
    public SkillTreeNodeSO Data;
    public bool IsPurchased = false;
    public bool IsUnlocked => PrevNodes.All(n => n.IsPurchased) || PrevNodes.Count == 0;
    public List<SkillTreeNode> PrevNodes = new();
    public List<SkillTreeNode> NextNodes = new();
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    public void ApplyEffects(SkillEffectContext context)
    {
        foreach (var effect in Data.effects)
        {
            effect.Apply(context);
        }
    }

    void OnButtonClicked()
    {
        SkillTreeManager.Instance.TryPurchase(this);
    }

    // TODO: ON HOVER TOOLTIP
}