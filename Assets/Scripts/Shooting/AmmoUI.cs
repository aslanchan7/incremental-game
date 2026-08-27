using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShootingModule shootingModule;
    [SerializeField] private GameObject ammoPrefab;
    private PlayerRuntimeStats playerRuntimeStats;

    [Header("Settings")]
    // [SerializeField] private Color activeAmmoColor;
    // [SerializeField] private Color unactiveAmmoColor;
    [SerializeField] private Sprite activeAmmoImg;
    [SerializeField] private Sprite inactiveAmmoImg;

    private List<Image> ammoImages = new();

    void Awake()
    {
        playerRuntimeStats = GameManager.Instance.PlayerRuntimeStats;
    }

    void Start()
    {
        for (int i = 0; i < playerRuntimeStats.MaxAmmo; i++)
        {
            GameObject instantiated = Instantiate(ammoPrefab, transform);
            Image imageComponent = instantiated.GetComponent<Image>();
            imageComponent.sprite = activeAmmoImg;
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
            ammoImages[i].sprite = (i >= ammo) ? inactiveAmmoImg : activeAmmoImg;
        }
    }
}
