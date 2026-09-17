using System;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "GunShopData", menuName = "Gun Shop/Gun Shop Data")]
public class GunShopData : ScriptableObject
{
    [SerializeField] public Dictionary<GunType, float> gunPrices;
}
