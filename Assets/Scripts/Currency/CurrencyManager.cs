using System;
using System.Collections.Generic;
using System.Linq;
using BreakInfinity;
using UnityEngine;

public class CurrencyManager : MonoBehaviour {
    public static CurrencyManager Instance;
    [SerializeField] private List<CurrencySO> allCurrencies;
    private Dictionary<string, CurrencySO> currencyDict;
    public Action<string> OnCurrencyChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        currencyDict = allCurrencies.ToDictionary(c => c.id, c => c);
    }

    public void Add(string currencyId, BigDouble amount)
    {
        if (currencyDict.TryGetValue(currencyId, out CurrencySO currency))
        {
            currency.Add(amount);
            OnCurrencyChanged?.Invoke(currencyId);
        } else
        {
            Debug.LogWarning($"Unkonwn currency id: {currencyId}");
        }
    }

    public bool TrySpend(string currencyId, BigDouble amount)
    {
        if (currencyDict.TryGetValue(currencyId, out CurrencySO currency))
        {
            bool success = currency.TrySpend(amount);
            if (success) OnCurrencyChanged?.Invoke(currencyId);
            return success;
        } else
        {
            Debug.LogWarning($"Unkonwn currency id: {currencyId}");
        }

        return false;
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