using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkillTreeNode : MonoBehaviour
{
    public SkillTreeNodeSO Data;
    [Space(10)]
    public List<SkillTreeNode> PrevNodes = new();
    public List<SkillTreeNode> NextNodes = new();
    [Space(10)]
    public bool IsPurchased = false;
    public bool IsUnlocked => PrevNodes.All(n => n.IsPurchased) || PrevNodes.Count == 0;
    public bool IsVisible => PrevNodes.Any(n => n.IsPurchased) || PrevNodes.Count == 0;
    [Space(10)]
    public Vector2Int GridPos;
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