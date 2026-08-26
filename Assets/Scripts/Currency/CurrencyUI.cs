using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private CurrencyManager currencyManager;

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
        cashText.text = $"${currencyManager.GetCurrency(currencyId).amount:F0}";
    }
}
