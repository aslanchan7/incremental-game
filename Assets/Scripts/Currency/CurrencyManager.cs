using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class CurrencyManager : MonoBehaviour {
    [SerializeField] private List<CurrencySO> allCurrencies;
    private Dictionary<string, CurrencySO> currencyDict;

    void Awake()
    {
        currencyDict = allCurrencies.ToDictionary(c => c.id, c => c);
    }

    public void Add(string currencyId, BigDouble amount)
    {
        if(currencyDict.TryGetValue(currencyId, out CurrencySO currency))
        {
            currency.Add(amount);
        } else
        {
            Debug.LogWarning($"Unkonwn currency id: {currencyId}");
        }
    }

    public CurrencySO GetCurrency(string currencyId)
    {
        return currencyDict[currencyId];
    }

    // public void LoadFrom(SaveData save) {
    //     foreach (var c in allCurrencies)
    //         c.amount = save.GetAmount(c.id);
    // }

    // public SaveData SaveTo() {
    //     var save = new SaveData();
    //     foreach (var c in allCurrencies)
    //         save.SetAmount(c.id, c.amount);
    //     return save;
    // }
}