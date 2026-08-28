using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        InitializePlayerRuntimeStats();
        InitializeRoundRuntimeData();
        SkillTree = new();
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
