using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [AutoStaticsCleanup] public static GameManager Instance = null;

    [Header("References")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private RoundData roundData;
    [SerializeField] public Dictionary<GunType, GameObject> GunDictionary;
    public GunType CurrGunType;
    [HideInInspector] public Gun CurrGunInstance;
    public PlayerRuntimeStats PlayerRuntimeStats;
    public RoundRuntimeData RoundRuntimeData;
    public UpgradesData UpgradesData;
    public SkillTree SkillTree;

    [Header("Debug")]
    public bool switchToSniper;
    public bool switchToPistol;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializePlayerRuntimeStats();
        InitializeRoundRuntimeData();
        InitializeUpgradesData();
        SkillTree = new SkillTree();
    }

    void Update()
    {
        if (switchToSniper)
        {
            switchToSniper = false;
            SwitchGun(GunType.Sniper);
        }

        if (switchToPistol)
        {
            switchToPistol = false;
            SwitchGun(GunType.Pistol);
        }
    }

    void InitializePlayerRuntimeStats()
    {
        PlayerRuntimeStats = new(playerData);

        // TODO: READ FROM SAVE FILE IF AVAILABLE
    }

    void InitializeRoundRuntimeData()
    {
        RoundRuntimeData = new(roundData);

        // TODO: READ FROM SAVE FILE IF AVAILABLE
    }

    void InitializeUpgradesData()
    {
        UpgradesData.Reset();

        // TODO: READ FROM SAVE FILE IF AVAILABLE
    }

    public void SwitchGun(GunType gunType)
    {
        CurrGunType = gunType;
    }
}
