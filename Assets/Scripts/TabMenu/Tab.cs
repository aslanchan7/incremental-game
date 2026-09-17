using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle), typeof(Image))]
public class Tab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image highlightImage;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;

    private void Reset()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        OnToggleValueChanged(toggle.isOn);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (highlightImage == null) return;

        highlightImage.color = toggle.isOn ? selectedColor : defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightImage == null) return;

        highlightImage.color = toggle.isOn ? selectedColor : hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightImage == null) return;

        highlightImage.color = toggle.isOn ? selectedColor : defaultColor;
    }
}
