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
        RoundManager.OnRoundEnd += Show;
        RoundManager.OnRoundStart += Hide;
        // CurrencyManager.Instance.OnCurrencyChanged += UpdateUI;
    }

    void OnDisable()
    {
        CurrencyManager.Instance.OnCurrencyChanged -= UpdateUI;
        RoundManager.OnRoundStart -= Hide;
        RoundManager.OnRoundEnd -= Show;
    }

    void UpdateUI(string currencyId)
    {
        cashText.text = $"${Mathf.FloorToInt((float)CurrencyManager.Instance.GetCurrency(currencyId).amount.ToDouble()):F0}";
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
