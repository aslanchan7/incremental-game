using BreakInfinity;
using UnityEngine;

public class GunShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunShopData gunShopData;
    
    public void PurchasePistol()
    {
        PurchaseGenericGun(GunType.Pistol);
    }

    public void PurchaseShotgun()
    {
        PurchaseGenericGun(GunType.Shotgun);
    }

    public void PurchaseSniper()
    {
        PurchaseGenericGun(GunType.Sniper);
    }

    private void PurchaseGenericGun(GunType gunType)
    {
        if (!GameManager.Instance.UnlockedGunTypes.Contains(gunType))
        {
            GameManager.Instance.UnlockedGunTypes.Add(gunType);
            
            bool hasCost = gunShopData.gunPrices.TryGetValue(gunType, out float cost);
            if (!hasCost)
            {
                Debug.LogError($"{gunType} does not have an associated cost in GunShopData");
                return;
            }

            CurrencyManager.Instance.TrySpend("cash", cost);
        }

        GameManager.Instance.SwitchGun(gunType);
    }
}
