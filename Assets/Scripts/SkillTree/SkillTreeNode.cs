using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkillTreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkillTreeNodeSO Data;
    [Space(10)]
    public List<SkillTreeNode> PrevNodes = new();
    public List<SkillTreeNode> NextNodes = new();
    [Space(10)]
    public bool IsPurchased = false;
    public bool IsDemoLocked = false;
    public bool IsUnlocked => PrevNodes.All(n => n.IsPurchased) || PrevNodes.Count == 0;
    public bool IsVisible => PrevNodes.Any(n => n.IsPurchased) || PrevNodes.Count == 0;
    [Space(10)]
    public Vector2Int GridPos;
    public Image SpriteImage;
    private Button button;
    private Vector3 origScale;

    void Awake()
    {
        button = GetComponent<Button>();
        SpriteImage = transform.GetChild(0).GetComponent<Image>();

        if (!IsDemoLocked)
            button.onClick.AddListener(OnButtonClicked);
        
        origScale = GetComponent<RectTransform>().localScale;
        InitializePrevNodes();
    }

    public void InitializePrevNodes()
    {
        foreach (var node in NextNodes)
        {
            if (node.PrevNodes.Contains(this)) continue;
            node.PrevNodes.Add(this);
        }
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillTreeTooltip.Instance.RequestShow(this);
        GetComponent<RectTransform>().localScale *= 1.1f;
        SFXManager.PlaySound(SoundType.UIHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SkillTreeTooltip.Instance.RequestHide();
        GetComponent<RectTransform>().localScale = origScale;
    }
}