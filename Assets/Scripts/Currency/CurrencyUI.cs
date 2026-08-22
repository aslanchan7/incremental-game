using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private TextMeshProUGUI cashText;

    void Update()
    {
        cashText.text = "$" + currencyManager.GetCurrency("cash").amount;
    }
}
