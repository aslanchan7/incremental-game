using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill Tree/Skill Tree Node")]
public class SkillTreeNodeSO : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public string displayName;
    public string description;
    public double cost;
    public List<SkillEffect> effects;
}
