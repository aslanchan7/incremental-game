using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SkillTreeManager))]
public class SkillTreeUI : MonoBehaviour, IDragHandler, IScrollHandler
{
    [Header("References")]
    [SerializeField] private RectTransform content; // the panel containing all nodes
    [SerializeField] private GameObject lineRendererPrefab;
    public Transform LineRendererParent;
    private SkillTreeManager skillTreeManager;
    [SerializeField] Sprite lockedNodeSprite;
    [SerializeField] RectTransform popupBox;
    private Coroutine popupBoxCoroutine;
    private List<SkillTreeNode> visibleNodes = new();

    [Header("Settings")]
    public float GridSpacing;
    [SerializeField] private Color purchasedNodeColor;
    [SerializeField] private Color unpurchasedNodeColor;
    [SerializeField] private Color lockedNodeColor;
    [SerializeField] private Color purchasedBorderColor;
    [SerializeField] private Color unpurchasedBorderColor;
    [SerializeField] private Color lockedBorderColor;

    [Header("Pan Config")]
    [SerializeField] private float panSpeed = 1f;
    [SerializeField] private Vector2 maxOffset;

    [Header("Zoom Config")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 2f;

    void Awake()
    {
        skillTreeManager = GetComponent<SkillTreeManager>();
    }
    
    void OnEnable()
    {
        skillTreeManager.OnNodePurchased += HandleNodePurchased;
        skillTreeManager.OnNodeDataInitialized += HandleNodeDataInitialized;

        // TODO: REMOVE THIS AFTER DEMO
        skillTreeManager.OnNodeLockedPurchaseAttempt += HandleNodeLocked;
        skillTreeManager.OnNotEnoughMoneyPurchaseAttempt += HandleNotEnoughMoney;
    }

    void OnDisable()
    {
        skillTreeManager.OnNodePurchased -= HandleNodePurchased;
        skillTreeManager.OnNodeDataInitialized -= HandleNodeDataInitialized;

        // TODO: REMOVE THIS AFTER DEMO
        skillTreeManager.OnNodeLockedPurchaseAttempt -= HandleNodeLocked;
        skillTreeManager.OnNotEnoughMoneyPurchaseAttempt -= HandleNotEnoughMoney;
    }

    void HandleNodeDataInitialized()
    {
        SkillTreeNode[] nodes = GetComponentsInChildren<SkillTreeNode>();
        InitializeLineRenderers(nodes);
        InitializeVisibleNodes(nodes);
    }    

    public void InitializeLineRenderers(SkillTreeNode[] nodes)
    {
        // Destroy existing line connectors
        UILineConnector[] connectors = LineRendererParent.GetComponentsInChildren<UILineConnector>();
        foreach (UILineConnector connector in connectors)
        {
            DestroyImmediate(connector.gameObject);
        }
        
        foreach (var node in nodes)
        {
            // Instantiate new line connectors
            for (int i = 0; i < node.NextNodes.Count; i++)
            {
                SkillTreeNode connectedNode = node.NextNodes[i];
                // GameObject instantiated = Instantiate(lineRendererPrefab, node.transform);
                GameObject instantiated = Instantiate(lineRendererPrefab, LineRendererParent);
                UILineConnector lineConnector = instantiated.GetComponent<UILineConnector>();
                lineConnector.transforms = new RectTransform[2];
                lineConnector.transforms[0] = node.GetComponent<RectTransform>();
                lineConnector.transforms[1] = connectedNode.GetComponent<RectTransform>();
            }
        }

        // UILineConnector[] connectors = lineRendererParent.GetComponentsInChildren<UILineConnector>();
        connectors = LineRendererParent.GetComponentsInChildren<UILineConnector>();
        foreach (var connector in connectors)
        {
            SkillTreeNode fromNode = connector.transforms[0].GetComponent<SkillTreeNode>();
            SkillTreeNode toNode = connector.transforms[1].GetComponent<SkillTreeNode>();
            connector.gameObject.SetActive(fromNode.IsVisible && toNode.IsVisible);
            
            if (toNode.IsDemoLocked)
            {
                connector.GetComponent<UILineRenderer>().color = lockedBorderColor;
                continue;
            }

            Color lineColor = lockedBorderColor;
            bool canAfford = CurrencyManager.Instance.GetCurrency("cash").amount >= toNode.Data.cost;
            if (toNode.IsPurchased)
            {
                lineColor = purchasedBorderColor;
            } else if (toNode.IsUnlocked)
            {
                lineColor = canAfford ? unpurchasedBorderColor : lockedBorderColor;
            }
            // Color lineColor = toNode.IsPurchased ? purchasedBorderColor : toNode.IsUnlocked ? unpurchasedBorderColor : lockedBorderColor;

            connector.GetComponent<UILineRenderer>().color = lineColor;
        }
    }

    void InitializeVisibleNodes(SkillTreeNode[] nodes)
    {
        foreach (var node in nodes)
        {
            node.gameObject.SetActive(node.IsVisible);
            if (node.IsVisible)
                visibleNodes.Add(node);

            // if (node.IsDemoLocked)
            // {
            //     node.GetComponent<Image>().color = lockedBorderColor; 
            //     node.SpriteImage.sprite = lockedNodeSprite;
            //     node.SpriteImage.color = lockedNodeColor;
            //     continue;
            // }
            
            // node.SpriteImage.sprite = node.Data.sprite;

            // bool canAfford = CurrencyManager.Instance.GetCurrency("cash").amount > node.Data.cost;
            // Color nodeBorderColor = lockedBorderColor;
            // Color spriteColor = lockedNodeColor;
            // if (node.IsPurchased)
            // {
            //     nodeBorderColor = purchasedBorderColor;
            //     spriteColor = purchasedNodeColor;
            // } else if (node.IsUnlocked)
            // {
            //     nodeBorderColor = canAfford ? unpurchasedBorderColor : lockedBorderColor;
            //     spriteColor = canAfford ? unpurchasedNodeColor : lockedNodeColor;
            // }

            // node.GetComponent<Image>().color = nodeBorderColor;
            // node.SpriteImage.color = spriteColor;
        }

        UpdateVisibleNodeVisuals();
    }

    void HandleNodePurchased(SkillTreeNode node)
    {
        node.GetComponent<Image>().color = purchasedBorderColor;
        node.SpriteImage.color = purchasedNodeColor;

        foreach (var connectedNode in node.NextNodes)
        {
            connectedNode.gameObject.SetActive(true);
            visibleNodes.Add(connectedNode);
        }

        UpdateVisibleNodeVisuals();

        // foreach (var connectedNode in node.NextNodes)
        // {
        //     connectedNode.gameObject.SetActive(true);
        //     // connectedNode.GetComponent<Image>().color = connectedNode.IsPurchased 
        //     //     ? purchasedBorderColor 
        //     //     : connectedNode.IsUnlocked 
        //     //         ? unpurchasedBorderColor 
        //     //         : lockedBorderColor;
        //     // connectedNode.SpriteImage.color = connectedNode.IsPurchased 
        //     //     ? purchasedNodeColor 
        //     //     : connectedNode.IsUnlocked 
        //     //         ? unpurchasedNodeColor 
        //     //         : lockedNodeColor;

        //     if (connectedNode.IsDemoLocked)
        //     {
        //         connectedNode.GetComponent<Image>().color = lockedBorderColor;          
        //         connectedNode.SpriteImage.color = lockedNodeColor;
        //     }

        //     bool canAfford = CurrencyManager.Instance.GetCurrency("cash").amount > connectedNode.Data.cost;
        //     Color nodeBorderColor = lockedBorderColor;
        //     Color spriteColor = lockedNodeColor;
        //     if (connectedNode.IsPurchased)
        //     {
        //         nodeBorderColor = purchasedBorderColor;
        //         spriteColor = purchasedNodeColor;
        //     } else if (connectedNode.IsUnlocked)
        //     {
        //         nodeBorderColor = canAfford ? unpurchasedBorderColor : lockedBorderColor;
        //         spriteColor = canAfford ? unpurchasedNodeColor : lockedNodeColor;
        //     }

        //     connectedNode.GetComponent<Image>().color = nodeBorderColor;
        //     connectedNode.SpriteImage.color = spriteColor;
        // }


        for (int i = 0; i < LineRendererParent.childCount; i++)
        {
            LineRendererParent.GetChild(i).TryGetComponent<UILineConnector>(out var connector);
            if(connector == null) continue;

            SkillTreeNode fromNode = connector.transforms[0].GetComponent<SkillTreeNode>();
            SkillTreeNode toNode = connector.transforms[1].GetComponent<SkillTreeNode>();
            connector.gameObject.SetActive(fromNode.IsVisible && toNode.IsVisible);
            
            if (toNode.IsDemoLocked)
            {
                connector.GetComponent<UILineRenderer>().color = lockedBorderColor;
                continue;
            }

            bool canAfford = CurrencyManager.Instance.GetCurrency("cash").amount >= toNode.Data.cost;
            Color lineColor = lockedBorderColor;
            if (toNode.IsPurchased)
            {
                lineColor = purchasedBorderColor;
            } else if (toNode.IsUnlocked)
            {
                lineColor = canAfford ? unpurchasedBorderColor : lockedBorderColor;
            }
            // Color lineColor = toNode.IsPurchased ? purchasedBorderColor : toNode.IsUnlocked ? unpurchasedBorderColor : lockedBorderColor;

            connector.GetComponent<UILineRenderer>().color = lineColor;
        }
    }

    private void UpdateVisibleNodeVisuals()
    {
        foreach (var node in visibleNodes)
        {
            if (node.IsDemoLocked)
            {
                node.GetComponent<Image>().color = lockedBorderColor; 
                node.SpriteImage.sprite = lockedNodeSprite;
                node.SpriteImage.color = lockedNodeColor;
                continue;
            }
            
            node.SpriteImage.sprite = node.Data.sprite;

            bool canAfford = CurrencyManager.Instance.GetCurrency("cash").amount >= node.Data.cost;
            Color nodeBorderColor = lockedBorderColor;
            Color spriteColor = lockedNodeColor;
            if (node.IsPurchased)
            {
                nodeBorderColor = purchasedBorderColor;
                spriteColor = purchasedNodeColor;
            } else if (node.IsUnlocked)
            {
                nodeBorderColor = canAfford ? unpurchasedBorderColor : lockedBorderColor;
                spriteColor = canAfford ? unpurchasedNodeColor : lockedNodeColor;
            }

            node.GetComponent<Image>().color = nodeBorderColor;
            node.SpriteImage.color = spriteColor;
        }
    }

    public void HandleContinueButton()
    {
        TransitionManager.Instance.StartFadeOutIn(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        content.anchoredPosition += eventData.delta * panSpeed;

        float clampedX = Mathf.Clamp(content.anchoredPosition.x, -maxOffset.x, maxOffset.x);
        float clampedY = Mathf.Clamp(content.anchoredPosition.y, -maxOffset.y, maxOffset.y);

        content.anchoredPosition = new(clampedX, clampedY);
    }

    public void OnScroll(PointerEventData eventData)
    {
        float zoomDelta = eventData.scrollDelta.y * zoomSpeed;
        float newScale = Mathf.Clamp(content.localScale.x + zoomDelta, minZoom, maxZoom);
        content.localScale = new Vector3(newScale, newScale, 1f);
    }

    private void HandleNodeLocked()
    {
        popupBox.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Node Is Not Unlocked";
        if (popupBoxCoroutine != null)
            StopCoroutine(popupBoxCoroutine);
        popupBoxCoroutine = StartCoroutine(ShowPopupBox());
    }

    private void HandleNotEnoughMoney()
    {
        popupBox.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Not Enough Money";
        if (popupBoxCoroutine != null)
            StopCoroutine(popupBoxCoroutine);
        popupBoxCoroutine = StartCoroutine(ShowPopupBox());
    }

    IEnumerator ShowPopupBox()
    {
        popupBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        popupBox.gameObject.SetActive(false);
        popupBoxCoroutine = null;
    }
}
