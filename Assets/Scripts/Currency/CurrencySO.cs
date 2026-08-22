using System;
using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "Currency", menuName = "Economy/Currency")]
public class CurrencySO : ScriptableObject {
    public string id;
    public string displayName;
    [NonSerialized] public BigDouble amount;
    public event Action<BigDouble> OnChanged;

    public void Add(BigDouble value) {
        amount += value;
        OnChanged?.Invoke(amount);
    }

    public bool TrySpend(BigDouble value) {
        if (amount < value) return false;
        amount -= value;
        OnChanged?.Invoke(amount);
        return true;
    }
}