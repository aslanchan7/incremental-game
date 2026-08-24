using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI cashText;
    private CurrencyManager currencyManager;

    void Awake()
    {
        currencyManager = GetComponent<CurrencyManager>();
    }

    void Start()
    {
        UpdateUI("cash");
    }

    void OnEnable()
    {
        currencyManager.OnCurrencyChanged += UpdateUI;
    }

    void OnDisable()
    {
        currencyManager.OnCurrencyChanged -= UpdateUI;
    }

    void UpdateUI(string currencyId)
    {
        cashText.text = $"${currencyManager.GetCurrency(currencyId).amount}";
    }
}
