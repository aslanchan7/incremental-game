using Unity.Scripting.LifecycleManagement;
using UnityEngine;

public partial class DebugManager : MonoBehaviour
{
    [AutoStaticsCleanup] public static DebugManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    [Header("Debug Options")]
    public bool AutoMapProgression;
    public bool AllowSkipRounds = true;
}
