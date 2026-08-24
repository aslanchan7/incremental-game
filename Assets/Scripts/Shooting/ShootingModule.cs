using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShootingModule : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private Image crosshair;
    private PlayerControls controls;
    private PlayerRuntimeStats playerRuntimeStats;

    [Header("Config")]
    [SerializeField] private float bullseyeDistanceThreshold;

    [Space(20)]
    private int currAmmo;
    [HideInInspector] public int CurrAmmo
    {
        get => currAmmo;
        set
        {
            if (currAmmo == value) return; // if value didn't change don't do anything
            currAmmo = value;
            OnAmmoValueChanged.Invoke(currAmmo);
        }
    }
    private bool isReloading;

    [Space(20)]

    [Header("Actions")]
    public Action<GameObject, bool> ShotFired;
    public Action<int> OnAmmoValueChanged;

    void Awake()
    {
        controls = new();
    }

    void Start()
    {
        playerRuntimeStats = GameManager.Instance.PlayerRuntimeStats;
        ShowCrosshair();
        CurrAmmo = playerRuntimeStats.MaxAmmo;
        crosshair.color = playerRuntimeStats.ActiveCrosshairColor;
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Shoot.performed += TryShoot;

        targetSpawner.OnTargetsCleared += HideCrosshair;
    }

    void OnDisable()
    {   
        controls.Player.Disable();
        controls.Player.Shoot.performed -= TryShoot;
    
        targetSpawner.OnTargetsCleared -= HideCrosshair;
    }

    void Update()
    {
        UpdateCrosshair();
    }

    void ShowCrosshair()
    {
        crosshair.gameObject.SetActive(true);
        Cursor.visible = false;
    }

    void HideCrosshair()
    {
        crosshair.gameObject.SetActive(false);
        Cursor.visible = true;        
    }

    void TryShoot(InputAction.CallbackContext context)
    {
        if (isReloading) return; // Can't shoot while reloading

        if (CurrAmmo == 0)
        {
            StartCoroutine(TryReload());
            return;   
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        var hit = Physics2D.GetRayIntersection(ray);

        GameObject target = null;
        bool isBullseye = false;
        
        if (hit.collider != null)
        {
            // Calculate the distance to the bullseye
            float distToBullseye = Vector2.Distance(worldPos, hit.collider.transform.position);
            isBullseye = distToBullseye < bullseyeDistanceThreshold;
            target = hit.collider.gameObject;
            
            Destroy(hit.collider.gameObject);
        }
  
        CurrAmmo--;
        ShotFired?.Invoke(target, isBullseye);

        if (CurrAmmo == 0)
        {
            StartCoroutine(TryReload());
            return;   
        }
    }

    IEnumerator TryReload()
    {
        yield return BasicAnimations.Interpolate(
            () =>
            {
                isReloading = true;
                crosshair.color = playerRuntimeStats.InactiveCrosshairColor;
            },
            (t) =>
            {
                crosshair.fillAmount = 1-t;
            },
            () =>
            {
                crosshair.fillAmount = 1f;
                CurrAmmo = playerRuntimeStats.MaxAmmo;
                isReloading = false;
                crosshair.color = playerRuntimeStats.ActiveCrosshairColor;
            },
            playerRuntimeStats.ReloadTime
        );
    }

    void UpdateCrosshair()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        crosshair.rectTransform.position = screenPos;
    }
}
