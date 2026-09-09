using Unity.Scripting.LifecycleManagement;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [AutoStaticsCleanup] public static GameManager Instance = null;

    [Header("References")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private RoundData roundData;
    [SerializeField] private GunDataSO currGunData;
    public PlayerRuntimeStats PlayerRuntimeStats;
    public RoundRuntimeData RoundRuntimeData;
    public SkillTree SkillTree;

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
        SkillTree = new SkillTree();
    }

    void InitializePlayerRuntimeStats()
    {
        PlayerRuntimeStats = new(playerData, currGunData);

        // TODO: READ FROM SAVE FILE IF AVAILABLE
    }

    void InitializeRoundRuntimeData()
    {
        RoundRuntimeData = new(roundData);

        // TODO: READ FROM SAVE FILE IF AVAILABLE
    }
}
