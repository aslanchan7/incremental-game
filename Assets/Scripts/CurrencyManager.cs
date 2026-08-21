using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour {
    [SerializeField] private List<CurrencySO> allCurrencies;

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