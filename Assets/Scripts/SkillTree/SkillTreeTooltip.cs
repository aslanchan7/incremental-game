using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeTooltip : MonoBehaviour
{
    public static SkillTreeTooltip Instance;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;
    [Space(10)]
    [SerializeField] private RectTransform nodesParent;
    [SerializeField] private SkillTreeUI skillTreeUI;

    [Header("Settings")]
    [SerializeField] private Vector2 tooltipOffset;

    private SkillTreeNode currHoveredNode;
    private Coroutine hideCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        } else
        {
            Instance = this;
        }
    }

    void Start()
    {
        HideTooltip();
    }

    void Update()
    {
        CalculatePosition();
    }

    public void RequestShow(SkillTreeNode node)
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        ShowTooltip(node);
    }

    public void RequestHide()
    {
        if (hideCoroutine != null) return; // hide tooltip has already been requested
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        HideTooltip();
        hideCoroutine = null;
    }

    private void ShowTooltip(SkillTreeNode node)
    {
        currHoveredNode = node;
        gameObject.SetActive(true);
        InitializeTooltipData();
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    void InitializeTooltipData()
    {
        title.text = currHoveredNode.Data.displayName;
        description.text = currHoveredNode.Data.description;
        cost.text = $"${currHoveredNode.Data.cost}";

        RectTransform tooltipRect = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);

    }

    void CalculatePosition()
    {
        RectTransform tooltipRect = GetComponent<RectTransform>();
        RectTransform hoveredNodeRect = currHoveredNode.GetComponent<RectTransform>();
        Vector2 hoveredNodeEdgePos = new(hoveredNodeRect.position.x,
            hoveredNodeRect.position.y + nodesParent.localScale.x * (hoveredNodeRect.sizeDelta.y / 2f));
        
        tooltipRect.position = hoveredNodeEdgePos + tooltipOffset;        
    }
}
