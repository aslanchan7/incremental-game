using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Vector3 origScale;

    void Awake()
    {
        origScale = GetComponent<RectTransform>().localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale *= 1.1f;
        SFXManager.PlaySound(SoundType.UIHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = origScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SFXManager.PlaySound(SoundType.UIClick);
    }
}
