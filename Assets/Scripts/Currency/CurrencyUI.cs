using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform cashContainer;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TargetSpawner targetSpawner;
    // [HideInInspector] public CurrencyManager CurrencyManager;

    void Awake()
    {
        // Hide();
    }

    void Start()
    {
        UpdateUI("cash");
        CurrencyManager.Instance.OnCurrencyChanged += UpdateUI;
    }

    void OnEnable()
    {
        // CurrencyManager.Instance.OnCurrencyChanged += UpdateUI;
        if (targetSpawner != null)
        {
            targetSpawner.OnTargetsCleared += Show;
            targetSpawner.OnRoundStart += Hide;
        }
    }

    void OnDisable()
    {
        CurrencyManager.Instance.OnCurrencyChanged -= UpdateUI;
        if (targetSpawner != null)
        {
            targetSpawner.OnTargetsCleared -= Show;
            targetSpawner.OnRoundStart -= Hide;  
        } 
    }

    void UpdateUI(string currencyId)
    {
        cashText.text = $"${CurrencyManager.Instance.GetCurrency(currencyId).amount:F0}";
    }

    public void Show()
    {
        cashContainer.gameObject.SetActive(true);
        UpdateUI("cash");
    }

    public void Hide()
    {
        cashContainer.gameObject.SetActive(false);
    }
}
