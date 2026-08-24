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
            UILineConnector lineConnector = node.transform.GetChild(0).GetComponent<UILineConnector>();
            lineConnector.transforms = new RectTransform[node.NextNodes.Count + 1];
            lineConnector.transforms[0] = node.GetComponent<RectTransform>();
            
            for (int i = 0; i < node.NextNodes.Count; i++)
            {
                SkillTreeNode connectedNode = node.NextNodes[i];
                lineConnector.transforms[i + 1] = connectedNode.GetComponent<RectTransform>();
            }
        }
    }

    void HandleNodePurchased(SkillTreeNode node)
    {
        // TODO: Update visuals when new skill is purchased/unlocked
    }

    public void HandleContinueButton()
    {
        SceneManager.LoadScene(0);
    }

    public void OnDrag(PointerEventData eventData) {
        content.anchoredPosition += eventData.delta * panSpeed;

        float clampedX = Mathf.Clamp(content.anchoredPosition.x, -maxOffset.x, maxOffset.x);
        float clampedY = Mathf.Clamp(content.anchoredPosition.y, -maxOffset.y, maxOffset.y);

        content.anchoredPosition = new(clampedX, clampedY);
    }

    public void OnScroll(PointerEventData eventData) {
        float zoomDelta = eventData.scrollDelta.y * zoomSpeed;
        float newScale = Mathf.Clamp(content.localScale.x + zoomDelta, minZoom, maxZoom);
        content.localScale = new Vector3(newScale, newScale, 1f);
    }
}
