using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShootingModule shootingModule;
    [SerializeField] private GameObject ammoPrefab;

    [Header("Settings")]
    [SerializeField] private Color activeAmmoColor;
    [SerializeField] private Color unactiveAmmoColor;

    private List<Image> ammoImages = new();

    void Start()
    {
        for (int i = 0; i < shootingModule.MaxAmmo; i++)
        {
            GameObject instantiated = Instantiate(ammoPrefab, transform);
            Image imageComponent = instantiated.GetComponent<Image>();
            imageComponent.color = activeAmmoColor;
            ammoImages.Add(imageComponent);
        }
    }

    void OnEnable()
    {
        shootingModule.OnAmmoValueChanged += UpdateAmmoUI;
    }

    void OnDisable()
    {
        shootingModule.OnAmmoValueChanged -= UpdateAmmoUI;
    }

    void UpdateAmmoUI(int ammo)
    {        
        for (int i = 0; i < ammoImages.Count; i++) {
            ammoImages[i].color = (i >= ammo) ? unactiveAmmoColor : activeAmmoColor;
        }
    }
}
