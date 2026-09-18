using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SceneData", menuName = "Scene/Scene Data")]
public class SceneData : ScriptableObject
{
    [SerializeField] public Dictionary<SceneType, int> sceneIndexDict;
}

public enum SceneType
{
    MainMenu,
    Shooting,
    Shop,
}