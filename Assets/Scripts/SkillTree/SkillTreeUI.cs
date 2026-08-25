using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(SkillTreeManager))]
public class SkillTreeUI : MonoBehaviour, IDragHandler, IScrollHandler
{
    [Header("References")]
    [SerializeField] private RectTransform content; // the panel containing all nodes
    [SerializeField] private GameObject lineRendererPrefab;
    private SkillTreeManager skillTreeManager;

    [Header("Settings")]
    public float GridSpacing;

    [Header("Pan Config")]
    [SerializeField] private float panSpeed = 1f;
    [SerializeField] private Vector2 maxOffset;

    [Header("Zoom Config")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 2f;

    void Awake()
    {
        // canvasGroup = GetComponent<CanvasGroup>();
        skillTreeManager = GetComponent<SkillTreeManager>();

        // HideSkillTree();
        SkillTreeNode[] nodes = GetComponentsInChildren<SkillTreeNode>();
        InitializeLineRenderers(nodes);
        InitializeVisibleNodes(nodes);
    }

    void OnEnable()
    {
        // targetSpawner.OnTargetsCleared += ShowSkillTree;
        skillTreeManager.OnNodePurchased += HandleNodePurchased;
    }

    void OnDisable()
    {
        // targetSpawner.OnTargetsCleared -= ShowSkillTree;
        skillTreeManager.OnNodePurchased -= HandleNodePurchased;
    }

    // void ShowSkillTree()
    // {
    //     canvasGroup.alpha = 1f;
    //     canvasGroup.interactable = true;
    // }

    // void HideSkillTree()
    // {
    //     canvasGroup.alpha = 0f;
    //     canvasGroup.interactable = false;
    // }

    public void InitializeLineRenderers(SkillTreeNode[] nodes)
    {
        foreach (var node in nodes)
        {
            // Destroy existing line connectors
            UILineConnector[] connectors = node.GetComponentsInChildren<UILineConnector>();
            foreach (UILineConnector connector in connectors)
            {
                DestroyImmediate(connector.gameObject);
            }

            // Instantiate new line connectors
            for (int i = 0; i < node.NextNodes.Count; i++)
            {
                SkillTreeNode connectedNode = node.NextNodes[i];
                GameObject instantiated = Instantiate(lineRendererPrefab, node.transform);
                UILineConnector lineConnector = instantiated.GetComponent<UILineConnector>();
                lineConnector.transforms = new RectTransform[2];
                lineConnector.transforms[0] = node.GetComponent<RectTransform>();
                lineConnector.transforms[1] = connectedNode.GetComponent<RectTransform>();
            }
        }
    }

    void InitializeVisibleNodes(SkillTreeNode[] nodes)
    {
        foreach (var node in nodes)
        {
            node.gameObject.SetActive(node.IsVisible);
            UILineConnector[] connectors = node.GetComponentsInChildren<UILineConnector>();
            foreach (var connector in connectors)
            {
                connector.gameObject.SetActive(connector.transforms[1].GetComponent<SkillTreeNode>().IsVisible);
            }
        }
    }

    void HandleNodePurchased(SkillTreeNode node)
    {
        // TODO: Update visuals when new skill is purchased/unlocked
        foreach (var connectedNode in node.NextNodes)
        {
            connectedNode.gameObject.SetActive(true);
        }

        for (int i = 0; i < node.transform.childCount; i++)
        {
            node.transform.GetChild(i).TryGetComponent<UILineConnector>(out var connector);
            if (connector != null)
            {
                connector.gameObject.SetActive(true);
            }
        }
        // UILineConnector[] connectors = node.GetComponentsInChildren<UILineConnector>();
        // foreach (var connector in connectors)
        // {
        //     connector.gameObject.SetActive(true);
        // }
    }

    public void HandleContinueButton()
    {
        SceneManager.LoadScene(0);
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
