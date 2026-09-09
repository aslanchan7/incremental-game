using System.Collections;
using UnityEngine;

public class DefaultTarget : Target
{
    void Awake()
    {
        BaseValue = GameManager.Instance.RoundRuntimeData.BaseTargetValue;
    }
}
