using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

[RequireComponent(typeof(SkillTreeManager))]
public class SkillTreeUI : MonoBehaviour, IDragHandler, IScrollHandler
{
    [Header("References")]
    [SerializeField] private RectTransform content; // the panel containing all nodes
    [SerializeField] private GameObject lineRendererPrefab;
    public Transform lineRendererParent;
    private SkillTreeManager skillTreeManager;

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
    }

    void OnDisable()
    {
        skillTreeManager.OnNodePurchased -= HandleNodePurchased;
        skillTreeManager.OnNodeDataInitialized -= HandleNodeDataInitialized;
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
        UILineConnector[] connectors = lineRendererParent.GetComponentsInChildren<UILineConnector>();
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
                GameObject instantiated = Instantiate(lineRendererPrefab, lineRendererParent);
                UILineConnector lineConnector = instantiated.GetComponent<UILineConnector>();
                lineConnector.transforms = new RectTransform[2];
                lineConnector.transforms[0] = node.GetComponent<RectTransform>();
                lineConnector.transforms[1] = connectedNode.GetComponent<RectTransform>();
            }
        }

        // UILineConnector[] connectors = lineRendererParent.GetComponentsInChildren<UILineConnector>();
        connectors = lineRendererParent.GetComponentsInChildren<UILineConnector>();
        foreach (var connector in connectors)
        {
            SkillTreeNode fromNode = connector.transforms[0].GetComponent<SkillTreeNode>();
            SkillTreeNode toNode = connector.transforms[1].GetComponent<SkillTreeNode>();
            connector.gameObject.SetActive(fromNode.IsVisible && toNode.IsVisible);
            Color lineColor = toNode.IsPurchased ? purchasedBorderColor : toNode.IsUnlocked ? unpurchasedBorderColor : lockedBorderColor;
            connector.GetComponent<UILineRenderer>().color = lineColor;
        }
    }

    void InitializeVisibleNodes(SkillTreeNode[] nodes)
    {
        foreach (var node in nodes)
        {
            node.gameObject.SetActive(node.IsVisible);
            node.SpriteImage.sprite = node.Data.sprite;
            node.GetComponent<Image>().color = node.IsPurchased ? purchasedBorderColor : node.IsUnlocked ? unpurchasedBorderColor : lockedBorderColor;
            node.SpriteImage.color = node.IsPurchased ? purchasedNodeColor : node.IsUnlocked ? unpurchasedNodeColor : lockedNodeColor;            
        }
    }

    void HandleNodePurchased(SkillTreeNode node)
    {
        node.GetComponent<Image>().color = purchasedBorderColor;
        node.SpriteImage.color = purchasedNodeColor;

        foreach (var connectedNode in node.NextNodes)
        {
            connectedNode.gameObject.SetActive(true);
            connectedNode.GetComponent<Image>().color = connectedNode.IsPurchased 
                ? purchasedBorderColor 
                : connectedNode.IsUnlocked 
                    ? unpurchasedBorderColor 
                    : lockedBorderColor;
            connectedNode.SpriteImage.color = connectedNode.IsPurchased 
                ? purchasedNodeColor 
                : connectedNode.IsUnlocked 
                    ? unpurchasedNodeColor 
                    : lockedNodeColor;            
        }


        for (int i = 0; i < lineRendererParent.childCount; i++)
        {
            lineRendererParent.GetChild(i).TryGetComponent<UILineConnector>(out var connector);
            if(connector == null) continue;

            SkillTreeNode fromNode = connector.transforms[0].GetComponent<SkillTreeNode>();
            SkillTreeNode toNode = connector.transforms[1].GetComponent<SkillTreeNode>();
            connector.gameObject.SetActive(fromNode.IsVisible && toNode.IsVisible);
            Color lineColor = toNode.IsPurchased ? purchasedBorderColor : toNode.IsUnlocked ? unpurchasedBorderColor : lockedBorderColor;
            connector.GetComponent<UILineRenderer>().color = lineColor;
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
}
