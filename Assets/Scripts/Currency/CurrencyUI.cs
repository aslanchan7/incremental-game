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
        Debug.Log("Enable");
    }

    void OnDisable()
    {
        currencyManager.OnCurrencyChanged -= UpdateUI;
        Debug.Log("Disable");
    }

    void UpdateUI(string currencyId)
    {
        Debug.Log("Update Cash UI");
        cashText.text = $"${currencyManager.GetCurrency(currencyId).amount:F0}";
    }
}
