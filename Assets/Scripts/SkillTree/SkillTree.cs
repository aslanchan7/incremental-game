using System;
using System.Collections.Generic;

[Serializable]
public class SkillTree
{
    public List<string> PurchasedNodeIds;

    public SkillTree()
    {
        PurchasedNodeIds = new();
    }
}
