using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShootingModule : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image crosshair;
    private PlayerControls controls;

    [Header("Config")]
    [SerializeField] private float bullseyeDistanceThreshold;

    [Space(20)]
    public int MaxAmmo;
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
    [SerializeField] private float reloadTime;
    private bool isReloading;
    [Space(20)]
    [SerializeField] private Color activeCrosshairColor;
    [SerializeField] private Color inactiveCrosshairColor;

    [Header("Actions")]
    public Action ShotFired;
    public Action<int> OnAmmoValueChanged;

    void Awake()
    {
        controls = new();
    }

    void Start()
    {
        Cursor.visible = false;
        CurrAmmo = MaxAmmo;
        crosshair.color = activeCrosshairColor;
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Shoot.performed += TryShoot;
    }

    void OnDisable()
    {   
        controls.Player.Disable();
        controls.Player.Shoot.performed -= TryShoot;
    }

    void Update()
    {
        UpdateCrosshair();
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

        if (hit.collider != null)
        {
            // Calculate the distance to the bullseye
            float distToBullseye = Vector2.Distance(worldPos, hit.collider.transform.position);
            bool isBullseye = distToBullseye < bullseyeDistanceThreshold ? true : false;

            // If bullseye give plus 50 money
            // if(isBullseye)
            // {
                
            // }
            
            Destroy(hit.collider.gameObject);
        }
  
        CurrAmmo--;
        ShotFired.Invoke();

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
                crosshair.color = inactiveCrosshairColor;
            },
            (t) =>
            {
                crosshair.fillAmount = 1-t;
            },
            () =>
            {
                crosshair.fillAmount = 1f;
                CurrAmmo = MaxAmmo;
                isReloading = false;
                crosshair.color = activeCrosshairColor;
            },
            reloadTime
        );
    }

    void UpdateCrosshair()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        crosshair.rectTransform.position = screenPos;
    }
}
